namespace Personal_Cabinet_Uni.ExternalInfoService.Models.DTO.Response;

public class EducationProgramResponse
{
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string Language { get; set; } = string.Empty;
    public string EducationForm { get; set; } = string.Empty;
    public FacultyResponse Faculty { get; set; } = new();
    public EducationLevelResponse EducationLevel { get; set; } = new();
}
