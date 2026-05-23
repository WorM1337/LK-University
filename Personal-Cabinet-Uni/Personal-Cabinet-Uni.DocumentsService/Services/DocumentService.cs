using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Personal_Cabinet_Uni.DocumentsService.Data;
using Personal_Cabinet_Uni.DocumentsService.Models.DTO.Request;
using Personal_Cabinet_Uni.DocumentsService.Models.DTO.Response;
using Personal_Cabinet_Uni.DocumentsService.Models.Entities;
using Personal_Cabinet_Uni.DocumentsService.Models.Enums;
using Personal_Cabinet_Uni.Shared.Exceptions;

namespace Personal_Cabinet_Uni.DocumentsService.Services;

public class DocumentService : IDocumentService
{
    private readonly DocumentsDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly DocumentStorageOptions _storageOptions;

    public DocumentService(
        DocumentsDbContext context,
        IWebHostEnvironment environment,
        IOptions<DocumentStorageOptions> storageOptions)
    {
        _context = context;
        _environment = environment;
        _storageOptions = storageOptions.Value;
    }

    public async Task<DocumentResponse> UploadAsync(UploadDocumentRequest request, string ownerEmail, CancellationToken cancellationToken = default)
    {
        if (request.File.Length == 0)
        {
            throw new BadRequestException("Файл пустой");
        }

        if (request.File.Length > _storageOptions.MaxFileSizeBytes)
        {
            throw new BadRequestException("Размер файла превышает допустимый лимит");
        }

        var id = Guid.NewGuid();
        var extension = Path.GetExtension(request.File.FileName);
        var ownerDirectory = NormalizePathPart(ownerEmail);
        var relativePath = Path.Combine(ownerDirectory, $"{id}{extension}");
        var absolutePath = GetAbsolutePath(relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        await using (var fileStream = File.Create(absolutePath))
        {
            await request.File.CopyToAsync(fileStream, cancellationToken);
        }

        var document = new DocumentRecord
        {
            Id = id,
            OwnerEmail = ownerEmail,
            DocumentType = request.DocumentType,
            Name = string.IsNullOrWhiteSpace(request.Name) ? Path.GetFileName(request.File.FileName) : request.Name.Trim(),
            OriginalFileName = Path.GetFileName(request.File.FileName),
            ContentType = string.IsNullOrWhiteSpace(request.File.ContentType) ? "application/octet-stream" : request.File.ContentType,
            RelativePath = relativePath,
            Size = request.File.Length,
            PassportSeries = request.PassportSeries,
            PassportNumber = request.PassportNumber,
            BirthPlace = request.BirthPlace,
            IssuedAt = NormalizeDateTime(request.IssuedAt),
            IssuedBy = request.IssuedBy,
            EducationDocumentName = request.EducationDocumentName,
            EducationLevelName = request.EducationLevelName,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Documents.AddAsync(document, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToResponse(document);
    }

    public async Task<IEnumerable<DocumentResponse>> GetAllAsync(string ownerEmail, DocumentType? documentType = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Documents
            .AsNoTracking()
            .Where(document => document.OwnerEmail == ownerEmail);

        if (documentType.HasValue)
        {
            query = query.Where(document => document.DocumentType == documentType.Value);
        }

        var documents = await query
            .OrderBy(document => document.DocumentType)
            .ThenBy(document => document.Name)
            .ToListAsync(cancellationToken);

        return documents.Select(MapToResponse);
    }

    public async Task<DocumentResponse> GetByIdAsync(Guid id, string requesterEmail, bool canReadAny, CancellationToken cancellationToken = default)
    {
        var document = await GetDocumentAsync(id, cancellationToken);
        EnsureAccess(document, requesterEmail, canReadAny);
        return MapToResponse(document);
    }

    public async Task<(Stream Stream, string ContentType, string FileName)> OpenFileAsync(Guid id, string requesterEmail, bool canReadAny, CancellationToken cancellationToken = default)
    {
        var document = await GetDocumentAsync(id, cancellationToken);
        EnsureAccess(document, requesterEmail, canReadAny);

        var absolutePath = GetAbsolutePath(document.RelativePath);
        if (!File.Exists(absolutePath))
        {
            throw new NotFoundException("Файл документа не найден");
        }

        var stream = File.OpenRead(absolutePath);
        return (stream, document.ContentType, document.OriginalFileName);
    }

    public async Task<DocumentResponse> UpdateMetadataAsync(Guid id, EditDocumentMetadataRequest request, string requesterEmail, bool canEditAny, CancellationToken cancellationToken = default)
    {
        var document = await GetDocumentAsync(id, cancellationToken);
        EnsureAccess(document, requesterEmail, canEditAny);

        if (request.DocumentType.HasValue)
            document.DocumentType = request.DocumentType.Value;
        if (!string.IsNullOrWhiteSpace(request.Name))
            document.Name = request.Name.Trim();

        document.PassportSeries = request.PassportSeries ?? document.PassportSeries;
        document.PassportNumber = request.PassportNumber ?? document.PassportNumber;
        document.BirthPlace = request.BirthPlace ?? document.BirthPlace;
        document.IssuedAt = NormalizeDateTime(request.IssuedAt) ?? document.IssuedAt;
        document.IssuedBy = request.IssuedBy ?? document.IssuedBy;
        document.EducationDocumentName = request.EducationDocumentName ?? document.EducationDocumentName;
        document.EducationLevelName = request.EducationLevelName ?? document.EducationLevelName;
        document.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return MapToResponse(document);
    }

    public async Task<DocumentResponse> RenameAsync(Guid id, string name, string requesterEmail, bool canEditAny, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BadRequestException("Название файла не может быть пустым");
        }

        var document = await GetDocumentAsync(id, cancellationToken);
        EnsureAccess(document, requesterEmail, canEditAny);

        document.Name = name.Trim();
        document.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return MapToResponse(document);
    }

    public async Task<DocumentResponse> DeleteAsync(Guid id, string requesterEmail, bool canEditAny, CancellationToken cancellationToken = default)
    {
        var document = await GetDocumentAsync(id, cancellationToken);
        EnsureAccess(document, requesterEmail, canEditAny);

        _context.Documents.Remove(document);
        await _context.SaveChangesAsync(cancellationToken);

        var absolutePath = GetAbsolutePath(document.RelativePath);
        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        return MapToResponse(document);
    }

    private async Task<DocumentRecord> GetDocumentAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Documents.FirstOrDefaultAsync(document => document.Id == id, cancellationToken)
            ?? throw new NotFoundException("Документ не найден");
    }

    private static void EnsureAccess(DocumentRecord document, string requesterEmail, bool canAccessAny)
    {
        if (!canAccessAny && !string.Equals(document.OwnerEmail, requesterEmail, StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenException("Доступ к документу запрещен");
        }
    }

    private string GetAbsolutePath(string relativePath)
    {
        var rootPath = Path.IsPathRooted(_storageOptions.RootPath)
            ? _storageOptions.RootPath
            : Path.Combine(_environment.ContentRootPath, _storageOptions.RootPath);

        return Path.Combine(rootPath, relativePath);
    }

    private static string NormalizePathPart(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var normalized = new string(value.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
        return normalized.Replace('@', '_').Replace('.', '_');
    }

    private static DateTime? NormalizeDateTime(DateTime? value)
    {
        return value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    private static DocumentResponse MapToResponse(DocumentRecord document)
    {
        return new DocumentResponse
        {
            Id = document.Id,
            OwnerEmail = document.OwnerEmail,
            DocumentType = document.DocumentType,
            Name = document.Name,
            OriginalFileName = document.OriginalFileName,
            ContentType = document.ContentType,
            Size = document.Size,
            Url = $"/document/{document.Id}/download",
            PassportSeries = document.PassportSeries,
            PassportNumber = document.PassportNumber,
            BirthPlace = document.BirthPlace,
            IssuedAt = document.IssuedAt,
            IssuedBy = document.IssuedBy,
            EducationDocumentName = document.EducationDocumentName,
            EducationLevelName = document.EducationLevelName,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }
}
