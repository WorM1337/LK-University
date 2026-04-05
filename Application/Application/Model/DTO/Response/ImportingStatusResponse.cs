using Application.Model.Enums;

namespace Application.Model.DTO.Response;

/// <summary>
/// Ответ - статус импорта справочника
/// </summary>
public class ImportingStatusResponse
{
    /// <summary>
    /// Статус импорта
    /// </summary>
    public DictionaryImportingStatus Status { get; set; }
}