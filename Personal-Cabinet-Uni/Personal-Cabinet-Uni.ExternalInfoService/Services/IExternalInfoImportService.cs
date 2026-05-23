using Personal_Cabinet_Uni.ExternalInfoService.Models.DTO.Response;

namespace Personal_Cabinet_Uni.ExternalInfoService.Services;

public interface IExternalInfoImportService
{
    Task<ImportStatusResponse> ImportEducationLevelsAsync(CancellationToken cancellationToken = default);
    Task<ImportStatusResponse> ImportDocumentTypesAsync(CancellationToken cancellationToken = default);
    Task<ImportStatusResponse> ImportFacultiesAsync(CancellationToken cancellationToken = default);
    Task<ImportStatusResponse> ImportProgramsAsync(CancellationToken cancellationToken = default);
    Task<ImportStatusResponse> GetStatusAsync(string dictionaryName, CancellationToken cancellationToken = default);
}
