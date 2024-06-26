﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Teams")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Team : IComparable<Team>
{
    [Key] [Column("team_id")] public int Id { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Место проживания")]
    [Column("location_id")]
    public int LocationId { get; set; }

    [Display(Name = "Место проживания")] public virtual Location? Location { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Название команды")]
    [Column("team_name")]
    [StringLength(60, ErrorMessage = "Текст должен быть меньше 60 символов.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Откуда команда")]
    [Column("come_from")]
    [StringLength(60, ErrorMessage = "Текст должен быть меньше 60 символов.")]
    public string ComeFrom { get; set; } = null!;

    [Display(Name = "Рейтинг команды")]
    [Range(0, int.MaxValue, ErrorMessage = "Число должно быть больше 0 и меньше 2147483647.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Число должно быть целым и положительным.")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("rating")]
    public int Rating { get; set; }

    [Display(Name = "Описание")]
    [Column("description")]
    public string? Description { get; set; }

    public override string ToString()
    {
        return Name;
    }

    public int CompareTo(Team? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }
}