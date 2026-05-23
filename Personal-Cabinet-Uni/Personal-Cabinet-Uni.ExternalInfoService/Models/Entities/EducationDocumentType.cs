namespace Personal_Cabinet_Uni.ExternalInfoService.Models.Entities;

public class EducationDocumentType
{
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public string Name { get; set; } = string.Empty;
    public int EducationLevelId { get; set; }
    public string NextEducationLevelIds { get; set; } = string.Empty;

    public EducationLevel? EducationLevel { get; set; }
}
