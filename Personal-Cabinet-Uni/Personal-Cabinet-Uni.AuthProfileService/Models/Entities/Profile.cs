using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.Models.Entities;

public class Profile
{
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
    public DateTime Birthday { get; set; }
    
    /// <summary>
    /// Пол
    /// </summary>
    public Gender Gender { get; set; }
    
    /// <summary>
    /// Национальность
    /// </summary>
    public string Nationality { get; set; }
    
    /// <summary>
    /// Хеш пароля
    /// </summary>
    public string PasswordHash { get; set; }
    
    /// <summary>
    /// Роль пользователя
    /// </summary>
    public Role Role { get; set; }
    
    /// <summary>
    /// Дата создания профиля
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Refresh токен
    /// </summary>
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// Срок действия refresh токена
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
