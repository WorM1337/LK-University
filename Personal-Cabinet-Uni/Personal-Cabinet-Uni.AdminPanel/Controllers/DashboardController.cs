using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Cabinet_Uni.AdminPanel.Controllers;

[Authorize(Policy = "AdminOnly")]
public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(ILogger<DashboardController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.UserName = User.Identity?.Name;
        ViewBag.UserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        return View();
    }
}
