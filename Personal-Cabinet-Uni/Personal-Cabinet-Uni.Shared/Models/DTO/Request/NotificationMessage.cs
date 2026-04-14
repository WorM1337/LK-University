namespace Personal_Cabinet_Uni.Shared.Models.DTO.Request;

/// <summary>
/// Сообщение для отправки уведомления
/// </summary>
public class NotificationMessage
{
    /// <summary>
    /// Адресат
    /// </summary>
    public string To { get; set; } = string.Empty;
    
    /// <summary>
    /// Тема сообщения
    /// </summary>
    public string Subject { get; set; } = string.Empty;
    
    /// <summary>
    /// Тело сообщения
    /// </summary>
    public string Body { get; set; } = string.Empty;
    
    /// <summary>
    /// Является ли сообщение HTML
    /// </summary>
    public bool IsHtml { get; set; } = false;
}
