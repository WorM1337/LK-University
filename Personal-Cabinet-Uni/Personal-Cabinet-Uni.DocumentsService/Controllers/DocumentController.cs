using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Personal_Cabinet_Uni.DocumentsService.Models.DTO.Request;
using Personal_Cabinet_Uni.DocumentsService.Models.DTO.Response;
using Personal_Cabinet_Uni.DocumentsService.Models.Enums;
using Personal_Cabinet_Uni.DocumentsService.Services;
using Personal_Cabinet_Uni.Shared.Exceptions;
using Personal_Cabinet_Uni.Shared.Models.DTO.Response;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.DocumentsService.Controllers;

[ApiController]
[Route("document")]
[Authorize]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    /// <summary>
    /// Загрузка скана документа.
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(DocumentResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request, CancellationToken cancellationToken)
    {
        var ownerEmail = ResolveOwnerEmail(request.OwnerEmail);
        var response = await _documentService.UploadAsync(request, ownerEmail, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Получение списка документов пользователя.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DocumentResponse>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    public async Task<IActionResult> GetDocuments(
        [FromQuery] string? ownerEmail = null,
        [FromQuery] DocumentType? documentType = null,
        CancellationToken cancellationToken = default)
    {
        var resolvedOwnerEmail = ResolveOwnerEmail(ownerEmail);
        var response = await _documentService.GetAllAsync(resolvedOwnerEmail, documentType, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Получение метаданных конкретного документа.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DocumentResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> GetDocument(Guid id, CancellationToken cancellationToken)
    {
        var response = await _documentService.GetByIdAsync(id, GetCurrentEmail(), CanReadAnyDocuments(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Скачивание файла документа.
    /// </summary>
    [HttpGet("{id:guid}/download")]
    [ProducesResponseType(typeof(FileResult), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> DownloadDocument(Guid id, CancellationToken cancellationToken)
    {
        var file = await _documentService.OpenFileAsync(id, GetCurrentEmail(), CanReadAnyDocuments(), cancellationToken);
        return File(file.Stream, file.ContentType, file.FileName);
    }

    /// <summary>
    /// Редактирование метаданных документа.
    /// </summary>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(DocumentResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> EditDocument(Guid id, [FromBody] EditDocumentMetadataRequest request, CancellationToken cancellationToken)
    {
        var response = await _documentService.UpdateMetadataAsync(id, request, GetCurrentEmail(), CanEditAnyDocuments(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Редактирование отображаемого имени файла.
    /// </summary>
    [HttpPatch("name/{id:guid}")]
    [ProducesResponseType(typeof(DocumentResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> EditDocumentName(Guid id, [FromQuery] string name, CancellationToken cancellationToken)
    {
        var response = await _documentService.RenameAsync(id, name, GetCurrentEmail(), CanEditAnyDocuments(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Удаление скана и его метаданных.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(DocumentResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> DeleteDocument(Guid id, CancellationToken cancellationToken)
    {
        var response = await _documentService.DeleteAsync(id, GetCurrentEmail(), CanEditAnyDocuments(), cancellationToken);
        return Ok(response);
    }

    private string ResolveOwnerEmail(string? requestedOwnerEmail)
    {
        var currentEmail = GetCurrentEmail();
        if (string.IsNullOrWhiteSpace(requestedOwnerEmail))
        {
            return currentEmail;
        }

        if (!CanEditAnyDocuments() && !string.Equals(currentEmail, requestedOwnerEmail, StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenException("Нельзя управлять документами другого пользователя");
        }

        return requestedOwnerEmail.Trim();
    }

    private string GetCurrentEmail()
    {
        return User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("email")
            ?? throw new UnauthorizedException("Необходима аутентификация");
    }

    private bool CanReadAnyDocuments()
    {
        return User.IsInRole(nameof(Role.Manager))
            || User.IsInRole(nameof(Role.MainManager))
            || User.IsInRole(nameof(Role.Admin));
    }

    private bool CanEditAnyDocuments()
    {
        return CanReadAnyDocuments();
    }
}
