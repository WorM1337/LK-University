using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Personal_Cabinet_Uni.AdminPanel.Models.DTO.Request;
using Personal_Cabinet_Uni.AdminPanel.Services;
using Personal_Cabinet_Uni.Shared.Models.Enums;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Personal_Cabinet_Uni.AdminPanel.Controllers;

public class AccountController : Controller
{
    private readonly IAuthServiceClient _authServiceClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAuthServiceClient authServiceClient,
        IConfiguration configuration,
        ILogger<AccountController> logger)
    {
        _authServiceClient = authServiceClient;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            var authResponse = await _authServiceClient.LoginAsync(request);

            if (authResponse == null)
            {
                ViewBag.Error = "Неверный email или пароль";
                return View(request);
            }

            var claims = GetClaimsFromJwt(authResponse.AccessToken);
            var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (roleClaim != nameof(Role.Admin))
            {
                ViewBag.Error = "Доступ запрещён. Требуется роль администратора.";
                return View(request);
            }

            HttpContext.Session.SetString("AccessToken", authResponse.AccessToken);
            HttpContext.Session.SetString("RefreshToken", authResponse.RefreshToken);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            });

            _logger.LogInformation("Админ {Email} успешно вошёл в систему", request.Email);

            return RedirectToAction("Index", "Dashboard");
        }
        catch (AuthServiceClientException ex)
        {
            _logger.LogWarning(ex, "Auth service rejected admin login for {Email}", request.Email);
            ViewBag.Error = ex.Message;
            return View(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при входе администратора {Email}", request.Email);
            ViewBag.Error = "Произошла ошибка при входе в систему";
            return View(request);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();

        _logger.LogInformation("Администратор вышел из системы");

        return RedirectToAction("Login");
    }

    private List<Claim> GetClaimsFromJwt(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(jwtToken);
        var claims = jwt.Claims.ToList();

        AddClaimIfMissing(claims, ClaimTypes.Email, "email");
        AddClaimIfMissing(claims, ClaimTypes.Name, "name");
        AddClaimIfMissing(claims, ClaimTypes.Role, "role");

        return claims;
    }

    private static void AddClaimIfMissing(List<Claim> claims, string targetType, string sourceType)
    {
        if (claims.Any(claim => claim.Type == targetType))
        {
            return;
        }

        var sourceClaim = claims.FirstOrDefault(claim => claim.Type == sourceType);
        if (sourceClaim != null)
        {
            claims.Add(new Claim(targetType, sourceClaim.Value));
        }
    }
}
