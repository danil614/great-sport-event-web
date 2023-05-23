﻿// Функция для обновления таблицы с очисткой кэша.
function refreshCache(controllerName) {
    refreshTableData(controllerName, null, null, true);
}

// Функция для обновления данных таблицы через AJAX.
function refreshTableData(controllerName, sortBy = null, sortDirection = null, clearCache = false) {
    if (sortBy == null || sortDirection == null) {
        // Получаем текущие значения сортировки
        let currentSortColumn = $("#dataTable th a.active-sort");
        sortBy = currentSortColumn.data("sort-by");
        sortDirection = currentSortColumn.data("sort-direction");
    }
    
    let table = $("#dataTable tbody");

    $.ajax({
        url: '/' + controllerName + '/GetSortedData',
        type: "GET",
        data: {
            sortBy: sortBy,
            sortDirection: sortDirection,
            clearCache: clearCache
        },
        success: function(data) {
            table.html(data);
        },
        error: function(error) {
            console.log(error);
            alert("Произошла ошибка при обновлении данных.");
        }
    });

    $("#searchInput").val("");
}

// Функция для обработки события клика на заголовке столбца.
function handleSortClick(controllerName, current) {
    // Удаляем классы сортировки у всех столбцов
    $("#dataTable th a").removeClass("active-sort");

    let sortBy = current.data("sort-by");
    let currentSortDirection = current.data("sort-direction");

    // Устанавливаем класс сортировки для текущего столбца
    current.addClass("active-sort");

    // Меняем направление сортировки
    let newSortDirection = currentSortDirection === "asc" ? "desc" : "asc";

    // Обновляем данные таблицы через AJAX
    refreshTableData(controllerName, sortBy, newSortDirection);

    // Обновляем значения атрибутов data-sort-direction
    current.data("sort-direction", newSortDirection);
}

// Функция для удаления записи по идентификатору.
function deleteItem(controllerName, id) {
    if (confirm("Вы уверены, что хотите удалить эту запись?")) {
        $.ajax({
            url: '/' + controllerName + '/DeleteItem',
            type: "POST",
            data: { id: id },
            success: function() {
                refreshTableData(controllerName);
                alert("Запись успешно удалена.");
            },
            error: function(error) {
                console.log(error);
                alert("Произошла ошибка при удалении записи.");
            }
        });
    }
}

// Функция для изменения записи по идентификатору.
function editItem(controllerName, id) {
    openModal('/' + controllerName + '/GetItem', id);
}

// Функция открытия модального окна.
function openModal(url, id) {
    // Получаем объект модального окна
    let modal = new bootstrap.Modal(document.getElementById('modalWindow'));
    
    $.ajax({
        url: url,
        type: "GET",
        data: { id: id },
        success: function (response) {
            $('#modalWindow').find('.modal-content').html(response);
            modal.show();
        },
        error: function(error) {
            console.log(error);
            alert("Произошла ошибка при загрузке данных.");
        }
    });
}

// Функция для создания новой записи.
function createItem(controllerName) {
    openModal('/' + controllerName + '/CreateItem', -1);
}

// Функция для поиска в таблице
function filterTable() {
    let searchText = $("#searchInput").val().trim().toLowerCase();
    let rows = $("#dataTable tbody tr");

    rows.each(function() {
        let row = $(this);
        let showRow = false;

        row.find("td").each(function() {
            let cellText = $(this).text().toLowerCase();
            if (cellText.includes(searchText)) {
                showRow = true;
                return false; // Exit the inner loop if a match is found in any cell
            }
        });

        if (showRow) {
            row.show();
        } else {
            row.hide();
        }
    });
}

// Функция для очистки поля поиска.
function clearInputFilter() {
    $("#searchInput").val("").focus();
    filterTable();
}

// Функция для проверки данных на уникальность.
function checkUnique(controllerName) {
    let formData = {};
    let fieldTags = ['input', 'textarea','select'];
    
    fieldTags.forEach(function (tag) {
        $('form ' + tag).each(function () {
            let fieldName = $(this).attr('name');
            formData[fieldName] = $(this).val();
        });
    });
    
    // Этот лог удалить
    console.log(formData);

    $.ajax({
        url: '/' + controllerName + '/CheckUnique',
        type: 'POST',
        data: JSON.stringify(formData),
        contentType: 'application/json',
        success: function (result) {
            if (result.isUnique) {
                $('form').unbind('submit').submit();
            } else {
                $('form input').addClass('input-validation-error');
                $('.text-danger').text('Запись не уникальна.');
                // А здесь нужно отредачить сообщение
            }
        }
    });
}
