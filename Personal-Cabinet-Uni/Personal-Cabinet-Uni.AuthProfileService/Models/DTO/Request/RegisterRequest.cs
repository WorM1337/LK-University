using System.ComponentModel.DataAnnotations;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.Models.DTO.Request;

/// <summary>
/// Запрос на регистрацию
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Имя
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Name { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Surname { get; set; }

    /// <summary>
    /// Отчество
    /// </summary>
    [MinLength(1)]
    public string LastName { get; set; }

    /// <summary>
    /// Адрес эл. почты
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// Пароль
    /// </summary>
    [Required]
    [MinLength(6)]
    public string Password { get; set; }

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
    [Required]
    public string Nationality { get; set; }
}
