namespace Personal_Cabinet_Uni.ExternalInfoService.Models.Entities;

public class EducationProgram
{
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string Language { get; set; } = string.Empty;
    public string EducationForm { get; set; } = string.Empty;
    public Guid FacultyId { get; set; }
    public int EducationLevelId { get; set; }

    public Faculty? Faculty { get; set; }
    public EducationLevel? EducationLevel { get; set; }
}
