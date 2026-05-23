using Microsoft.EntityFrameworkCore;
using Personal_Cabinet_Uni.ExternalInfoService.Data;
using Personal_Cabinet_Uni.ExternalInfoService.Models.DTO.External;
using Personal_Cabinet_Uni.ExternalInfoService.Models.DTO.Response;
using Personal_Cabinet_Uni.ExternalInfoService.Models.Entities;
using Personal_Cabinet_Uni.Shared.Exceptions;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.ExternalInfoService.Services;

public class ExternalInfoImportService : IExternalInfoImportService
{
    private readonly ExternalInfoDbContext _dbContext;
    private readonly IExternalDictionaryClient _client;
    private readonly ILogger<ExternalInfoImportService> _logger;

    public ExternalInfoImportService(
        ExternalInfoDbContext dbContext,
        IExternalDictionaryClient client,
        ILogger<ExternalInfoImportService> logger)
    {
        _dbContext = dbContext;
        _client = client;
        _logger = logger;
    }

    public async Task<ImportStatusResponse> ImportEducationLevelsAsync(CancellationToken cancellationToken = default)
    {
        return await ImportAsync(ExternalDictionaryNames.EducationLevels, async () =>
        {
            var levels = await _client.GetEducationLevelsAsync(cancellationToken);
            UpsertEducationLevels(levels);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var actualIds = levels.Select(x => x.Id).ToHashSet();
            var obsoleteLevels = await _dbContext.EducationLevels
                .Where(x => !actualIds.Contains(x.Id))
                .Where(x => !_dbContext.EducationDocumentTypes.Any(d => d.EducationLevelId == x.Id))
                .Where(x => !_dbContext.EducationPrograms.Any(p => p.EducationLevelId == x.Id))
                .ToListAsync(cancellationToken);
            _dbContext.EducationLevels.RemoveRange(obsoleteLevels);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return levels.Count;
        }, cancellationToken);
    }

    public async Task<ImportStatusResponse> ImportDocumentTypesAsync(CancellationToken cancellationToken = default)
    {
        return await ImportAsync(ExternalDictionaryNames.DocumentTypes, async () =>
        {
            var documentTypes = await _client.GetDocumentTypesAsync(cancellationToken);
            UpsertEducationLevels(documentTypes.Select(x => x.EducationLevel)
                .Concat(documentTypes.SelectMany(x => x.NextEducationLevels)));
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _dbContext.Database.ExecuteSqlRawAsync(
                $"TRUNCATE TABLE {ExternalInfoDbContext.SchemaName}.education_document_types",
                cancellationToken);

            _dbContext.EducationDocumentTypes.AddRange(documentTypes.Select(ToEntity));
            await _dbContext.SaveChangesAsync(cancellationToken);
            return documentTypes.Count;
        }, cancellationToken);
    }

    public async Task<ImportStatusResponse> ImportFacultiesAsync(CancellationToken cancellationToken = default)
    {
        return await ImportAsync(ExternalDictionaryNames.Faculties, async () =>
        {
            var faculties = await _client.GetFacultiesAsync(cancellationToken);
            UpsertFaculties(faculties);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var actualIds = faculties.Select(x => x.Id).ToHashSet();
            var obsoleteFaculties = await _dbContext.Faculties
                .Where(x => !actualIds.Contains(x.Id))
                .Where(x => !_dbContext.EducationPrograms.Any(p => p.FacultyId == x.Id))
                .ToListAsync(cancellationToken);
            _dbContext.Faculties.RemoveRange(obsoleteFaculties);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return faculties.Count;
        }, cancellationToken);
    }

    public async Task<ImportStatusResponse> ImportProgramsAsync(CancellationToken cancellationToken = default)
    {
        return await ImportAsync(ExternalDictionaryNames.Programs, async () =>
        {
            var programs = await _client.GetProgramsAsync(cancellationToken);
            UpsertEducationLevels(programs.Select(x => x.EducationLevel));
            UpsertFaculties(programs.Select(x => x.Faculty));
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _dbContext.Database.ExecuteSqlRawAsync(
                $"TRUNCATE TABLE {ExternalInfoDbContext.SchemaName}.education_programs",
                cancellationToken);

            _dbContext.EducationPrograms.AddRange(programs.Select(ToEntity));
            await _dbContext.SaveChangesAsync(cancellationToken);
            return programs.Count;
        }, cancellationToken);
    }

    public async Task<ImportStatusResponse> GetStatusAsync(string dictionaryName, CancellationToken cancellationToken = default)
    {
        var status = await _dbContext.ImportStatuses.AsNoTracking()
            .FirstOrDefaultAsync(x => x.DictionaryName == dictionaryName, cancellationToken);

        if (status == null)
        {
            throw new NotFoundException($"Статус импорта {dictionaryName} не найден");
        }

        return ToResponse(status);
    }

    private async Task<ImportStatusResponse> ImportAsync(
        string dictionaryName,
        Func<Task<int>> importAction,
        CancellationToken cancellationToken)
    {
        await SetStatusAsync(dictionaryName, DictionaryImportingStatus.Processing, 0, null, cancellationToken);

        try
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            var importedCount = await importAction();
            await transaction.CommitAsync(cancellationToken);

            return await SetStatusAsync(dictionaryName, DictionaryImportingStatus.Success, importedCount, null, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при импорте справочника {DictionaryName}", dictionaryName);
            return await SetStatusAsync(dictionaryName, DictionaryImportingStatus.Failed, 0, ex.Message, cancellationToken);
        }
    }

    private async Task<ImportStatusResponse> SetStatusAsync(
        string dictionaryName,
        DictionaryImportingStatus status,
        int importedCount,
        string? errorMessage,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ImportStatuses
            .FirstOrDefaultAsync(x => x.DictionaryName == dictionaryName, cancellationToken);

        if (entity == null)
        {
            entity = new DictionaryImportStatus { DictionaryName = dictionaryName };
            _dbContext.ImportStatuses.Add(entity);
        }

        entity.Status = status;
        entity.ImportedCount = importedCount;
        entity.ErrorMessage = errorMessage;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(entity);
    }

    private void UpsertEducationLevels(IEnumerable<ExternalEducationLevelDto> levels)
    {
        foreach (var level in levels.GroupBy(x => x.Id).Select(x => x.First()))
        {
            var entity = _dbContext.EducationLevels.Local.FirstOrDefault(x => x.Id == level.Id)
                ?? _dbContext.EducationLevels.Find(level.Id);

            if (entity == null)
            {
                _dbContext.EducationLevels.Add(ToEntity(level));
            }
            else
            {
                entity.Name = level.Name;
            }
        }
    }

    private void UpsertFaculties(IEnumerable<ExternalFacultyDto> faculties)
    {
        foreach (var faculty in faculties.GroupBy(x => x.Id).Select(x => x.First()))
        {
            var entity = _dbContext.Faculties.Local.FirstOrDefault(x => x.Id == faculty.Id)
                ?? _dbContext.Faculties.Find(faculty.Id);

            if (entity == null)
            {
                _dbContext.Faculties.Add(ToEntity(faculty));
            }
            else
            {
                entity.Name = faculty.Name;
                entity.CreateTime = ToUtc(faculty.CreateTime);
            }
        }
    }

    private static EducationLevel ToEntity(ExternalEducationLevelDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name
    };

    private static Faculty ToEntity(ExternalFacultyDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        CreateTime = ToUtc(dto.CreateTime)
    };

    private static EducationDocumentType ToEntity(ExternalEducationDocumentTypeDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        CreateTime = ToUtc(dto.CreateTime),
        EducationLevelId = dto.EducationLevel.Id,
        NextEducationLevelIds = string.Join(",", dto.NextEducationLevels.Select(x => x.Id))
    };

    private static EducationProgram ToEntity(ExternalEducationProgramDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Code = dto.Code,
        Language = dto.Language,
        EducationForm = dto.EducationForm,
        FacultyId = dto.Faculty.Id,
        EducationLevelId = dto.EducationLevel.Id,
        CreateTime = ToUtc(dto.CreateTime)
    };

    private static ImportStatusResponse ToResponse(DictionaryImportStatus status) => new()
    {
        DictionaryName = status.DictionaryName,
        Status = status.Status,
        ImportedCount = status.ImportedCount,
        ErrorMessage = status.ErrorMessage,
        UpdatedAt = status.UpdatedAt
    };

    private static DateTime ToUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
