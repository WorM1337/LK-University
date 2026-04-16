using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Personal_Cabinet_Uni.AdminPanel.Models.DTO.Request;
using Personal_Cabinet_Uni.AdminPanel.Services;
using Personal_Cabinet_Uni.Models.DTO.Response;
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
            return View(managers ?? Enumerable.Empty<ProfileResponse>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка менеджеров");
            return View(Enumerable.Empty<ProfileResponse>());
        }
    }

    [HttpGet("{email}")]
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
            return NotFound();
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Roles = Enum.GetValues(typeof(Role)).Cast<Role>().Where(r => r == Role.Manager || r == Role.MainManager).ToList();
        ViewBag.Genders = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
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
            ViewBag.Roles = Enum.GetValues(typeof(Role)).Cast<Role>().Where(r => r == Role.Manager || r == Role.MainManager).ToList();
            ViewBag.Genders = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
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

            TempData["Error"] = "Не удалось создать менеджера";
            return View(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании менеджера {Email}", request.Email);
            ViewBag.Roles = Enum.GetValues(typeof(Role)).Cast<Role>().Where(r => r == Role.Manager || r == Role.MainManager).ToList();
            ViewBag.Genders = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
            return View(request);
        }
    }

    [HttpGet("{email}/Edit")]
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

            ViewBag.Roles = Enum.GetValues(typeof(Role)).Cast<Role>().Where(r => r == Role.Manager || r == Role.MainManager).ToList();
            ViewBag.Genders = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
            ViewBag.Email = email;

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении информации о менеджере {Email}", email);
            return NotFound();
        }
    }

    [HttpPost("{email}/Edit")]
    public async Task<IActionResult> Edit(string email, EditManagerRequest request)
    {
        var token = GetAdminToken();
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Roles = Enum.GetValues(typeof(Role)).Cast<Role>().Where(r => r == Role.Manager || r == Role.MainManager).ToList();
            ViewBag.Genders = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
            ViewBag.Email = email;
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

            TempData["Error"] = "Не удалось обновить данные менеджера";
            ViewBag.Roles = Enum.GetValues(typeof(Role)).Cast<Role>().Where(r => r == Role.Manager || r == Role.MainManager).ToList();
            ViewBag.Genders = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
            ViewBag.Email = email;
            return View(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении данных менеджера {Email}", email);
            ViewBag.Roles = Enum.GetValues(typeof(Role)).Cast<Role>().Where(r => r == Role.Manager || r == Role.MainManager).ToList();
            ViewBag.Genders = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
            ViewBag.Email = email;
            return View(request);
        }
    }

    [HttpPost("{email}/Delete")]
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении менеджера {Email}", email);
            return RedirectToAction("Index", "Managers");
        }
    }

    private string? GetAdminToken()
    {
        return HttpContext.Session.GetString("AccessToken");
    }
}
