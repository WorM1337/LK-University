using Personal_Cabinet_Uni.DocumentsService.Models.DTO.Request;
using Personal_Cabinet_Uni.DocumentsService.Models.DTO.Response;
using Personal_Cabinet_Uni.DocumentsService.Models.Enums;

namespace Personal_Cabinet_Uni.DocumentsService.Services;

public interface IDocumentService
{
    Task<DocumentResponse> UploadAsync(UploadDocumentRequest request, string ownerEmail, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentResponse>> GetAllAsync(string ownerEmail, DocumentType? documentType = null, CancellationToken cancellationToken = default);
    Task<DocumentResponse> GetByIdAsync(Guid id, string requesterEmail, bool canReadAny, CancellationToken cancellationToken = default);
    Task<(Stream Stream, string ContentType, string FileName)> OpenFileAsync(Guid id, string requesterEmail, bool canReadAny, CancellationToken cancellationToken = default);
    Task<DocumentResponse> UpdateMetadataAsync(Guid id, EditDocumentMetadataRequest request, string requesterEmail, bool canEditAny, CancellationToken cancellationToken = default);
    Task<DocumentResponse> RenameAsync(Guid id, string name, string requesterEmail, bool canEditAny, CancellationToken cancellationToken = default);
    Task<DocumentResponse> DeleteAsync(Guid id, string requesterEmail, bool canEditAny, CancellationToken cancellationToken = default);
}
