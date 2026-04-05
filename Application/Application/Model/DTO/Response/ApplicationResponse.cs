using Application.Model.Enums;

namespace Application.Model.DTO.Response;

/// <summary>
/// Ответ-информация о заявке на поступление абитуриента
/// </summary>
public class ApplicationResponse
{
    /// <summary>
    /// ID заявки
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// ID абитуриента
    /// </summary>
    public string ApplicantEmail { get; set; }
    /// <summary>
    /// Статус заявки
    /// </summary>
    public EnteringStatus EnteringStatus { get; set; }
}