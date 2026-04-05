namespace Application.Model.DTO.Response;

/// <summary>
/// Ответ на загрузку файла
/// </summary>
public class DocumentResponse
{
    /// <summary>
    /// URL файла
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// Название файла
    /// </summary>
    public string Name { get; set; }
}