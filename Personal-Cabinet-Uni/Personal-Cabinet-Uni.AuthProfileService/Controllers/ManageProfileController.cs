using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Personal_Cabinet_Uni.Models.DTO.Request;
using Personal_Cabinet_Uni.Models.DTO.Response;
using Personal_Cabinet_Uni.Services;
using Personal_Cabinet_Uni.Shared.Exceptions;
using Personal_Cabinet_Uni.Shared.Models.DTO.Response;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.Controllers;

/// <summary>
/// Контроллер профилей
/// </summary>
[ApiController]
[Route("profile")]
public class ManageProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ManageProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    /// <summary>
    /// Получение профилей с пагинацией
    /// </summary>
    /// <param name="page">Страница</param>
    /// <param name="limit">Макс. количество элементов</param>
    /// <param name="role">Роль. При null выбираются все роли</param>
    /// <returns>Список профилей</returns>
    /// <remarks>В зависимости от параметров фильтрации есть разные права. Например менеджер не может посмотреть список менеджеров</remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProfileResponse>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [Authorize(Roles = $"{nameof(Role.Manager)},{nameof(Role.MainManager)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> GetProfiles(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] Role? role = null,
        CancellationToken cancellationToken = default)
    {
        var profiles = await _profileService.GetAllAsync(page, limit, role, cancellationToken);
        return Ok(profiles);
    }

    /// <summary>
    /// Получить конкретный профиль по email
    /// </summary>
    /// <param name="email">Адрес эл. почты</param>
    /// <returns>Профиль</returns>
    [HttpGet("{email}")]
    [ProducesResponseType(typeof(ProfileResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [Authorize]
    public async Task<IActionResult> GetProfile([EmailAddress] string email, CancellationToken cancellationToken)
    {
        var userEmail = User.Identity?.Name;
        var userRole = User.FindFirst("Role")?.Value;

        if (string.IsNullOrEmpty(userEmail))
        {
            throw new UnauthorizedException("Необходима аутентификация");
        }

        if (!string.Equals(email, userEmail, StringComparison.OrdinalIgnoreCase) && 
            string.Equals(userRole, nameof(Role.Appplicant), StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenException("Доступ запрещен");
        }

        var profile = await _profileService.GetByEmailAsync(email, cancellationToken);
        if (profile == null)
        {
            throw new NotFoundException("Профиль не найден");
        }
        return Ok(profile);
    }

    /// <summary>
    /// Изменение профиля по email
    /// </summary>
    /// <param name="email">Адрес эл. почты</param>
    /// <param name="request">Параметры для изменения</param>
    /// <returns>Изменённый профиль</returns>
    [HttpPatch("{email}")]
    [ProducesResponseType(typeof(ProfileResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [Authorize]
    public async Task<IActionResult> EditProfile(
        string email,
        [FromBody] EditProfileRequest request,
        CancellationToken cancellationToken)
    {
        var profile = await _profileService.UpdateAsync(email, request, cancellationToken);
        return Ok(profile);
    }

    /// <summary>
    /// Сбрасывание пароля
    /// </summary>
    /// <param name="email">Адрес эл. почты</param>
    [HttpPatch("{email}/resetPassword")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Authorize]
    public async Task<IActionResult> ResetPassword(string email, CancellationToken cancellationToken)
    {
        await _profileService.ResetPasswordAsync(email, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Создание менеджера
    /// </summary>
    /// <param name="request">Данные менеджера</param>
    [HttpPost("manager")]
    [ProducesResponseType(typeof(ProfileResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [Authorize(Roles = $"{nameof(Role.MainManager)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> CreateManager(
        [FromBody] CreateManagerRequest request,
        CancellationToken cancellationToken)
    {
        var profile = await _profileService.CreateManagerAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetProfile), new { email = profile.Email }, profile);
    }

    /// <summary>
    /// Обновление менеджера
    /// </summary>
    /// <param name="email">Адрес эл. почты</param>
    /// <param name="request">Параметры для изменения</param>
    [HttpPatch("manager/{email}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [Authorize(Roles = $"{nameof(Role.MainManager)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> UpdateManager(
        string email,
        [FromBody] EditManagerRequest request,
        CancellationToken cancellationToken)
    {
        await _profileService.UpdateManagerAsync(email, request, cancellationToken);
        return Ok();
    }
}
