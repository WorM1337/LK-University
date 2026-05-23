namespace Personal_Cabinet_Uni.ExternalInfoService.Models.DTO.External;

public class ExternalProgramPagedListDto
{
    public List<ExternalEducationProgramDto> Programs { get; set; } = [];
    public ExternalPaginationDto Pagination { get; set; } = new();
}
