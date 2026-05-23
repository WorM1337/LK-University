namespace Personal_Cabinet_Uni.ExternalInfoService.Models.DTO.Response;

public class EducationDocumentTypeResponse
{
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public string Name { get; set; } = string.Empty;
    public EducationLevelResponse EducationLevel { get; set; } = new();
    public List<EducationLevelResponse> NextEducationLevels { get; set; } = [];
}
