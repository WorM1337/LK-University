using System.ComponentModel.DataAnnotations;
using Application.Model.Enums;

namespace Application.Model.DTO.Request;

/// <summary>
/// Запрос на изменение профиля менеджера
/// </summary>
public class EditManagerRequest
{
    /// <summary>
    /// Имя
    /// </summary>
    [MinLength(1)]
    public string Name { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    [MinLength(1)]
    public string Surname { get; set; }

    /// <summary>
    /// Отчество
    /// </summary>
    [MinLength(1)]
    public string LastName { get; set; }

    /// <summary>
    /// Телефон
    /// </summary>
    [RegularExpression(@"^\+7(\d){10}$")]
    public string Phone { get; set; }

    /// <summary>
    /// Дата рождения
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// Пол
    /// </summary>
    public Gender? Gender { get; set; }
    
    /// <summary>
    /// Национальность
    /// </summary>
    [MinLength(1)]
    public string Nationality { get; set; }
    
    
}