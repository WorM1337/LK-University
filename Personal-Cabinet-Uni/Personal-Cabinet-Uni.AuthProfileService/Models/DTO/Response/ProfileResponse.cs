using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.Models.DTO.Response;

/// <summary>
/// Ответ-информация о профиле пользователя
/// </summary>
public class ProfileResponse
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public string Surname { get; set; }

    /// <summary>
    /// Отчество
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Адрес эл. почты
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Телефон
    /// </summary>
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
    public string Nationality { get; set; }

    /// <summary>
    /// Роль
    /// </summary>
    public Role Role { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
