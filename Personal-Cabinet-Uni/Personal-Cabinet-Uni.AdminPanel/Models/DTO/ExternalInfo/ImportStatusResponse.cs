using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.AdminPanel.Models.DTO.ExternalInfo;

public class ImportStatusResponse
{
    public string DictionaryName { get; set; } = string.Empty;
    public DictionaryImportingStatus Status { get; set; }
    public int ImportedCount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
