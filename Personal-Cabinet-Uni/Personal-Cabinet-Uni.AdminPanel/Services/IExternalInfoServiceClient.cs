using Personal_Cabinet_Uni.AdminPanel.Models.DTO.ExternalInfo;

namespace Personal_Cabinet_Uni.AdminPanel.Services;

public interface IExternalInfoServiceClient
{
    Task<IReadOnlyCollection<ImportStatusResponse>> GetStatusesAsync(string adminToken, CancellationToken cancellationToken = default);
    Task<ImportStatusResponse?> ImportEducationLevelsAsync(string adminToken, CancellationToken cancellationToken = default);
    Task<ImportStatusResponse?> ImportDocumentTypesAsync(string adminToken, CancellationToken cancellationToken = default);
    Task<ImportStatusResponse?> ImportFacultiesAsync(string adminToken, CancellationToken cancellationToken = default);
    Task<ImportStatusResponse?> ImportProgramsAsync(string adminToken, CancellationToken cancellationToken = default);
}
