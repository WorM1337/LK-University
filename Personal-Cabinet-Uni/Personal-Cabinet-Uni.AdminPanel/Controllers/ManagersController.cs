using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Personal_Cabinet_Uni.AdminPanel.Models.DTO.Request;
using Personal_Cabinet_Uni.AdminPanel.Models.DTO.Response;
using Personal_Cabinet_Uni.AdminPanel.Services;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.AdminPanel.Controllers;

[Authorize(Policy = "AdminOnly")]
public class ManagersController : Controller
{
    private readonly IAuthServiceClient _authServiceClient;
    private readonly ILogger<ManagersController> _logger;

    public ManagersController(
        IAuthServiceClient authServiceClient,
        ILogger<ManagersController> logger)
    {
        _authServiceClient = authServiceClient;
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
            var managers = await _authServiceClient.GetAllManagersAsync(token);
            return View(managers ?? Enumerable.Empty<ManagerResponse>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка менеджеров");
            TempData["Error"] = "Не удалось получить список менеджеров";
            return View(Enumerable.Empty<ManagerResponse>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(string email)
    {
        var token = GetAdminToken();
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var manager = await _authServiceClient.GetManagerByEmailAsync(email, token);
            if (manager == null)
            {
                return NotFound();
            }
            return View(manager);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении информации о менеджере {Email}", email);
            TempData["Error"] = "Не удалось получить информацию о менеджере";
            return NotFound();
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        FillSelectLists();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateManagerRequest request)
    {
        var token = GetAdminToken();
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            FillSelectLists();
            return View(request);
        }

        try
        {
            var result = await _authServiceClient.CreateManagerAsync(request, token);
            if (result != null)
            {
                TempData["Success"] = $"Менеджер {result.Surname} {result.Name} успешно создан";
                return RedirectToAction("Index", "Managers");
            }

            TempData["Error"] = "Auth service не вернул созданного менеджера";
            return View(request);
        }
        catch (AuthServiceClientException ex)
        {
            _logger.LogWarning(ex, "Auth service rejected manager creation {Email}", request.Email);
            FillSelectLists();
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании менеджера {Email}", request.Email);
            FillSelectLists();
            ModelState.AddModelError(string.Empty, "Произошла ошибка при создании менеджера");
            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string email)
    {
        var token = GetAdminToken();
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var manager = await _authServiceClient.GetManagerByEmailAsync(email, token);
            if (manager == null)
            {
                return NotFound();
            }

            var model = new EditManagerRequest
            {
                Name = manager.Name,
                Surname = manager.Surname,
                LastName = manager.LastName,
                Phone = manager.Phone,
                Birthday = manager.Birthday,
                Gender = manager.Gender,
                Nationality = manager.Nationality,
                Role = manager.Role
            };

            FillSelectLists(email);

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении информации о менеджере {Email}", email);
            TempData["Error"] = "Не удалось открыть менеджера для редактирования";
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string email, EditManagerRequest request)
    {
        var token = GetAdminToken();
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            FillSelectLists(email);
            return View(request);
        }

        try
        {
            var result = await _authServiceClient.EditManagerAsync(email, request, token);
            if (result != null)
            {
                TempData["Success"] = $"Данные менеджера {result.Surname} {result.Name} успешно обновлены";
                return RedirectToAction("Index", "Managers");
            }

            TempData["Error"] = "Auth service не вернул обновленного менеджера";
            FillSelectLists(email);
            return View(request);
        }
        catch (AuthServiceClientException ex)
        {
            _logger.LogWarning(ex, "Auth service rejected manager update {Email}", email);
            FillSelectLists(email);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении данных менеджера {Email}", email);
            FillSelectLists(email);
            ModelState.AddModelError(string.Empty, "Произошла ошибка при обновлении менеджера");
            return View(request);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string email)
    {
        var token = GetAdminToken();
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var result = await _authServiceClient.DeleteManagerAsync(email, token);
            if (result)
            {
                TempData["Success"] = $"Менеджер {email} успешно удалён";
                return RedirectToAction("Index", "Managers");
            }

            TempData["Error"] = "Не удалось удалить менеджера";
            return RedirectToAction("Index", "Managers");
        }
        catch (AuthServiceClientException ex)
        {
            _logger.LogWarning(ex, "Auth service rejected manager delete {Email}", email);
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Managers");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении менеджера {Email}", email);
            TempData["Error"] = "Произошла ошибка при удалении менеджера";
            return RedirectToAction("Index", "Managers");
        }
    }

    private string? GetAdminToken()
    {
        return HttpContext.Session.GetString("AccessToken");
    }

    private void FillSelectLists(string? email = null)
    {
        ViewBag.Roles = Enum.GetValues(typeof(Role)).Cast<Role>().Where(r => r == Role.Manager || r == Role.MainManager).ToList();
        ViewBag.Genders = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
        ViewBag.Email = email;
    }
}
