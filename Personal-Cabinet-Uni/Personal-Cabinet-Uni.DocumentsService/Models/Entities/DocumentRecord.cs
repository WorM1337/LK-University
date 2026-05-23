using Personal_Cabinet_Uni.DocumentsService.Models.Enums;

namespace Personal_Cabinet_Uni.DocumentsService.Models.Entities;

public class DocumentRecord
{
    public Guid Id { get; set; }
    public string OwnerEmail { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public string RelativePath { get; set; } = string.Empty;
    public long Size { get; set; }
    public string? PassportSeries { get; set; }
    public string? PassportNumber { get; set; }
    public string? BirthPlace { get; set; }
    public DateTime? IssuedAt { get; set; }
    public string? IssuedBy { get; set; }
    public string? EducationDocumentName { get; set; }
    public string? EducationLevelName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
