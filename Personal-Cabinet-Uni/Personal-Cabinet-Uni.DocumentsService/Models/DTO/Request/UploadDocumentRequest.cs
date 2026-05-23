using System.ComponentModel.DataAnnotations;
using Personal_Cabinet_Uni.DocumentsService.Models.Enums;

namespace Personal_Cabinet_Uni.DocumentsService.Models.DTO.Request;

public class UploadDocumentRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;

    [Required]
    public DocumentType DocumentType { get; set; }

    [MinLength(1)]
    public string? Name { get; set; }

    [EmailAddress]
    public string? OwnerEmail { get; set; }

    public string? PassportSeries { get; set; }
    public string? PassportNumber { get; set; }
    public string? BirthPlace { get; set; }
    public DateTime? IssuedAt { get; set; }
    public string? IssuedBy { get; set; }
    public string? EducationDocumentName { get; set; }
    public string? EducationLevelName { get; set; }
}
