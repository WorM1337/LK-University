namespace Personal_Cabinet_Uni.ExternalInfoService.Models.DTO.External;

public class ExternalEducationDocumentTypeDto
{
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public string Name { get; set; } = string.Empty;
    public ExternalEducationLevelDto EducationLevel { get; set; } = new();
    public List<ExternalEducationLevelDto> NextEducationLevels { get; set; } = [];
}
