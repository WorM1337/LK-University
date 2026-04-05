namespace Application.Model.DTO.Response;

/// <summary>
/// Подробный ответ с заявкой абитуриента
/// </summary>
public class DetailedApplicationResponse : ApplicationResponse
{
    public IEnumerable<ProgramResponseWithPriority> Programs { get; set; }
    public IEnumerable<DocumentResponse> Documents { get; set; }
}