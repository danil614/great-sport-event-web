﻿using GreatSportEventWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Type = GreatSportEventWeb.Models.Type;

namespace GreatSportEventWeb.Controllers;

public class TypesController : Controller
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationContext _context;

    public TypesController(ApplicationContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(DatabaseScripts<Type>.GetCachedData(_context, _cache));
    }

    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache) _cache.Remove(typeof(Type));

        var data = DatabaseScripts<Type>.GetCachedData(_context, _cache);

        data = sortBy switch
        {
            "name" => sortDirection == "asc" 
                ? data.OrderBy(item => item.Name) 
                : data.OrderByDescending(item => item.Name),
            _ => data
        };

        return PartialView("_Table", data);
    }

    [HttpPost]
    public IActionResult DeleteItem(int id)
    {
        var item = _context.Types.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        _context.Types.Remove(item);
        var rowsAffected = _context.SaveChanges();

        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(typeof(Type));

        return rowsAffected > 0 ? Ok() : StatusCode(500);
    }

    [HttpGet]
    public IActionResult GetItem(int id)
    {
        var item = _context.Types.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404
        ViewBag.Edit = true;

        return PartialView("Form", item);
    }

    [HttpGet]
    public IActionResult CreateItem()
    {
        var item = new Type();
        ViewBag.Edit = false;
        return PartialView("Form", item);
    }

    [HttpPost]
    public IActionResult SaveItem(Type item)
    {
        if (ModelState.IsValid)
        {
            _context.Types.Update(item);
            var rowsAffected = _context.SaveChanges();

            // При обновлении записи в базе данных, очищаем кэш
            _cache.Remove(typeof(Type));

            return rowsAffected > 0 ? Redirect("/Types") : StatusCode(500);
        }
        
        return StatusCode(500);
    }

    [HttpGet]
    public FileContentResult ExportToExcel()
    {
        var data = DatabaseScripts<Type>.GetCachedData(_context, _cache).ToList();
        string[] columns = { "Name" };
        var fileContent = ExcelExport.ExportExcel(data, columns, true);
        return File(fileContent ?? Array.Empty<byte>(), ExcelExport.ExcelContentType, "Types.xlsx");
    }
}