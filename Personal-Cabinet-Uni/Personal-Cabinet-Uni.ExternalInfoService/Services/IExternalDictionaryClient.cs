using Personal_Cabinet_Uni.ExternalInfoService.Models.DTO.External;

namespace Personal_Cabinet_Uni.ExternalInfoService.Services;

public interface IExternalDictionaryClient
{
    Task<List<ExternalEducationLevelDto>> GetEducationLevelsAsync(CancellationToken cancellationToken = default);
    Task<List<ExternalEducationDocumentTypeDto>> GetDocumentTypesAsync(CancellationToken cancellationToken = default);
    Task<List<ExternalFacultyDto>> GetFacultiesAsync(CancellationToken cancellationToken = default);
    Task<List<ExternalEducationProgramDto>> GetProgramsAsync(CancellationToken cancellationToken = default);
}
