using System.ComponentModel.DataAnnotations;

namespace Application.Model.DTO.Request;

/// <summary>
/// Запрос на логин
/// </summary>
public class LoginRequest
{
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
}