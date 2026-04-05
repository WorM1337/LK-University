namespace Application.Model.DTO.Response;

/// <summary>
/// Ответ на ошибку
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Статус код ошибки
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string Message { get; set; }
}