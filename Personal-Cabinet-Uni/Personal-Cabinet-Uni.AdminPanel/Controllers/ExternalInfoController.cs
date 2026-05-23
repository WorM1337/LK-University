using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Personal_Cabinet_Uni.AdminPanel.Models.DTO.ExternalInfo;
using Personal_Cabinet_Uni.AdminPanel.Services;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.AdminPanel.Controllers;

[Authorize(Policy = "AdminOnly")]
public class ExternalInfoController : Controller
{
    private readonly IExternalInfoServiceClient _externalInfoServiceClient;
    private readonly ILogger<ExternalInfoController> _logger;

    public ExternalInfoController(
        IExternalInfoServiceClient externalInfoServiceClient,
        ILogger<ExternalInfoController> logger)
    {
        _externalInfoServiceClient = externalInfoServiceClient;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var token = GetAdminToken();
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var statuses = await _externalInfoServiceClient.GetStatusesAsync(token);
            return View(BuildViewModel(statuses));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении статусов импорта справочников");
            TempData["Error"] = "Не удалось получить статусы импорта справочников";
            return View(BuildViewModel([]));
        }
    }

    [HttpPost]
    public Task<IActionResult> ImportEducationLevels()
    {
        return ImportAsync(
            client => client.ImportEducationLevelsAsync(GetAdminToken()!),
            "Уровни образования импортированы");
    }

    [HttpPost]
    public Task<IActionResult> ImportDocumentTypes()
    {
        return ImportAsync(
            client => client.ImportDocumentTypesAsync(GetAdminToken()!),
            "Типы документов импортированы");
    }

    [HttpPost]
    public Task<IActionResult> ImportFaculties()
    {
        return ImportAsync(
            client => client.ImportFacultiesAsync(GetAdminToken()!),
            "Факультеты импортированы");
    }

    [HttpPost]
    public Task<IActionResult> ImportPrograms()
    {
        return ImportAsync(
            client => client.ImportProgramsAsync(GetAdminToken()!),
            "Образовательные программы импортированы");
    }

    private async Task<IActionResult> ImportAsync(
        Func<IExternalInfoServiceClient, Task<ImportStatusResponse?>> importAction,
        string successMessage)
    {
        var token = GetAdminToken();
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var result = await importAction(_externalInfoServiceClient);
            if (result?.Status == DictionaryImportingStatus.Success)
            {
                TempData["Success"] = $"{successMessage}. Загружено записей: {result.ImportedCount}";
            }
            else
            {
                TempData["Error"] = result?.ErrorMessage ?? "Импорт завершился с ошибкой";
            }
        }
        catch (ExternalInfoServiceClientException ex)
        {
            _logger.LogWarning(ex, "External info service rejected dictionary import");
            TempData["Error"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при импорте справочника");
            TempData["Error"] = "Не удалось импортировать справочник";
        }

        return RedirectToAction("Index");
    }

    private string? GetAdminToken()
    {
        return HttpContext.Session.GetString("AccessToken");
    }

    private static ExternalInfoIndexViewModel BuildViewModel(IReadOnlyCollection<ImportStatusResponse> statuses)
    {
        var statusesByName = statuses.ToDictionary(x => x.DictionaryName, StringComparer.OrdinalIgnoreCase);

        return new ExternalInfoIndexViewModel
        {
            Dictionaries =
            [
                BuildDictionary("educationLevels", "Уровни образования", "ImportEducationLevels", statusesByName),
                BuildDictionary("documentTypes", "Типы документов", "ImportDocumentTypes", statusesByName),
                BuildDictionary("faculties", "Факультеты", "ImportFaculties", statusesByName),
                BuildDictionary("programs", "Образовательные программы", "ImportPrograms", statusesByName)
            ]
        };
    }

    private static ExternalDictionaryViewModel BuildDictionary(
        string key,
        string title,
        string importAction,
        IReadOnlyDictionary<string, ImportStatusResponse> statuses)
    {
        statuses.TryGetValue(key, out var status);

        return new ExternalDictionaryViewModel
        {
            Key = key,
            Title = title,
            ImportAction = importAction,
            Status = status?.Status ?? DictionaryImportingStatus.Failed,
            ImportedCount = status?.ImportedCount ?? 0,
            ErrorMessage = status?.ErrorMessage,
            UpdatedAt = status?.UpdatedAt
        };
    }
}
