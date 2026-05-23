namespace Personal_Cabinet_Uni.ExternalInfoService.Models.DTO.External;

public class ExternalEducationProgramDto
{
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string Language { get; set; } = string.Empty;
    public string EducationForm { get; set; } = string.Empty;
    public ExternalFacultyDto Faculty { get; set; } = new();
    public ExternalEducationLevelDto EducationLevel { get; set; } = new();
}
