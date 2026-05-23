using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Personal_Cabinet_Uni.ExternalInfoService.Data;
using Personal_Cabinet_Uni.ExternalInfoService.Models.DTO.Response;
using Personal_Cabinet_Uni.ExternalInfoService.Services;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.ExternalInfoService.Controllers;

[ApiController]
[Route("externalInfo")]
[Authorize(Roles = nameof(Role.Admin))]
public class ExternalInfoController : ControllerBase
{
    private readonly ExternalInfoDbContext _dbContext;
    private readonly IExternalInfoImportService _importService;

    public ExternalInfoController(ExternalInfoDbContext dbContext, IExternalInfoImportService importService)
    {
        _dbContext = dbContext;
        _importService = importService;
    }

    [HttpPost("educationLevels")]
    public Task<ImportStatusResponse> ImportEducationLevels(CancellationToken cancellationToken)
    {
        return _importService.ImportEducationLevelsAsync(cancellationToken);
    }

    [HttpGet("educationLevels/status")]
    public Task<ImportStatusResponse> GetEducationLevelsStatus(CancellationToken cancellationToken)
    {
        return _importService.GetStatusAsync(ExternalDictionaryNames.EducationLevels, cancellationToken);
    }

    [HttpGet("educationLevels")]
    public async Task<IEnumerable<EducationLevelResponse>> GetEducationLevels(CancellationToken cancellationToken)
    {
        return await _dbContext.EducationLevels.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new EducationLevelResponse { Id = x.Id, Name = x.Name })
            .ToListAsync(cancellationToken);
    }

    [HttpPost("documentTypes")]
    public Task<ImportStatusResponse> ImportDocumentTypes(CancellationToken cancellationToken)
    {
        return _importService.ImportDocumentTypesAsync(cancellationToken);
    }

    [HttpGet("documentTypes/status")]
    public Task<ImportStatusResponse> GetDocumentTypesStatus(CancellationToken cancellationToken)
    {
        return _importService.GetStatusAsync(ExternalDictionaryNames.DocumentTypes, cancellationToken);
    }

    [HttpGet("documentTypes")]
    public async Task<IEnumerable<EducationDocumentTypeResponse>> GetDocumentTypes(CancellationToken cancellationToken)
    {
        var levels = await _dbContext.EducationLevels.AsNoTracking()
            .ToDictionaryAsync(x => x.Id, x => new EducationLevelResponse { Id = x.Id, Name = x.Name }, cancellationToken);

        var documentTypes = await _dbContext.EducationDocumentTypes.AsNoTracking()
            .Include(x => x.EducationLevel)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return documentTypes.Select(x => new EducationDocumentTypeResponse
        {
            Id = x.Id,
            CreateTime = x.CreateTime,
            Name = x.Name,
            EducationLevel = new EducationLevelResponse
            {
                Id = x.EducationLevelId,
                Name = x.EducationLevel?.Name ?? string.Empty
            },
            NextEducationLevels = x.NextEducationLevelIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id, out var value) && levels.TryGetValue(value, out var level) ? level : null)
                .Where(x => x != null)
                .Cast<EducationLevelResponse>()
                .ToList()
        });
    }

    [HttpPost("faculties")]
    public Task<ImportStatusResponse> ImportFaculties(CancellationToken cancellationToken)
    {
        return _importService.ImportFacultiesAsync(cancellationToken);
    }

    [HttpGet("faculties/status")]
    public Task<ImportStatusResponse> GetFacultiesStatus(CancellationToken cancellationToken)
    {
        return _importService.GetStatusAsync(ExternalDictionaryNames.Faculties, cancellationToken);
    }

    [HttpGet("faculties")]
    public async Task<IEnumerable<FacultyResponse>> GetFaculties(CancellationToken cancellationToken)
    {
        return await _dbContext.Faculties.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new FacultyResponse
            {
                Id = x.Id,
                CreateTime = x.CreateTime,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);
    }

    [HttpPost("programs")]
    public Task<ImportStatusResponse> ImportPrograms(CancellationToken cancellationToken)
    {
        return _importService.ImportProgramsAsync(cancellationToken);
    }

    [HttpGet("programs/status")]
    public Task<ImportStatusResponse> GetProgramsStatus(CancellationToken cancellationToken)
    {
        return _importService.GetStatusAsync(ExternalDictionaryNames.Programs, cancellationToken);
    }

    [HttpGet("programs")]
    public async Task<IEnumerable<EducationProgramResponse>> GetPrograms(CancellationToken cancellationToken)
    {
        return await _dbContext.EducationPrograms.AsNoTracking()
            .Include(x => x.Faculty)
            .Include(x => x.EducationLevel)
            .OrderBy(x => x.Name)
            .Select(x => new EducationProgramResponse
            {
                Id = x.Id,
                CreateTime = x.CreateTime,
                Name = x.Name,
                Code = x.Code,
                Language = x.Language,
                EducationForm = x.EducationForm,
                Faculty = new FacultyResponse
                {
                    Id = x.FacultyId,
                    CreateTime = x.Faculty != null ? x.Faculty.CreateTime : default,
                    Name = x.Faculty != null ? x.Faculty.Name : string.Empty
                },
                EducationLevel = new EducationLevelResponse
                {
                    Id = x.EducationLevelId,
                    Name = x.EducationLevel != null ? x.EducationLevel.Name : string.Empty
                }
            })
            .ToListAsync(cancellationToken);
    }
}
