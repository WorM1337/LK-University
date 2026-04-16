using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Personal_Cabinet_Uni.Models.DTO.Request;
using Personal_Cabinet_Uni.Models.DTO.Response;
using Personal_Cabinet_Uni.Services;
using Personal_Cabinet_Uni.Shared.Models.DTO.Response;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="request">Данные на регистрацию</param>
    /// <returns>Объект с JWT токеном</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Login), response);
    }

    /// <summary>
    /// Логин пользователя
    /// </summary>
    /// <param name="request">Логин и пароль</param>
    /// <returns>Объект с JWT токеном</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Обновление токенов
    /// </summary>
    /// <param name="request">Access и refresh токены</param>
    /// <returns>Объект с новыми токенами</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RefreshTokenAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Создание менеджера (доступно только админу)
    /// </summary>
    /// <param name="request">Данные менеджера</param>
    /// <returns>Созданный профиль менеджера</returns>
    [HttpPost("manager")]
    [Authorize(Roles = nameof(Role.Admin))]
    [ProducesResponseType(typeof(ProfileResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    public async Task<IActionResult> CreateManager([FromBody] CreateManagerRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.CreateManagerAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetManagerByEmail), new { email = response.Email }, response);
    }

    /// <summary>
    /// Редактирование менеджера (доступно только админу)
    /// </summary>
    /// <param name="email">Email менеджера</param>
    /// <param name="request">Данные для редактирования</param>
    /// <returns>Обновленный профиль менеджера</returns>
    [HttpPatch("manager/{email}")]
    [Authorize(Roles = nameof(Role.Admin))]
    [ProducesResponseType(typeof(ProfileResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> EditManager(string email, [FromBody] EditManagerRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.EditManagerAsync(email, request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Удаление менеджера (доступно только админу)
    /// </summary>
    /// <param name="email">Email менеджера</param>
    /// <returns>Результат удаления</returns>
    [HttpDelete("manager/{email}")]
    [Authorize(Roles = nameof(Role.Admin))]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> DeleteManager(string email, CancellationToken cancellationToken)
    {
        await _authService.DeleteManagerAsync(email, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Получение всех менеджеров (доступно только админу)
    /// </summary>
    /// <returns>Список менеджеров</returns>
    [HttpGet("managers")]
    [Authorize(Roles = nameof(Role.Admin))]
    [ProducesResponseType(typeof(IEnumerable<ProfileResponse>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    public async Task<IActionResult> GetAllManagers(CancellationToken cancellationToken)
    {
        var response = await _authService.GetAllManagersAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Получение менеджера по email (доступно только админу)
    /// </summary>
    /// <param name="email">Email менеджера</param>
    /// <returns>Профиль менеджера</returns>
    [HttpGet("manager/{email}")]
    [Authorize(Roles = nameof(Role.Admin))]
    [ProducesResponseType(typeof(ProfileResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> GetManagerByEmail(string email, CancellationToken cancellationToken)
    {
        var response = await _authService.GetManagerByEmailAsync(email, cancellationToken);
        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }
}
