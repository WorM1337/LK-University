using System.ComponentModel.DataAnnotations;
using Application.Model.DTO.Request;
using Application.Model.DTO.Response;
using Application.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

/// <summary>
/// Контроллер профилей
/// </summary>
[ApiController]
[Route("profile")]
public class ManageProfileController : ControllerBase
{
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
    public IActionResult GetProfiles(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] Role? role = null)
    {
        return NoContent();
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
    public IActionResult GetProfile([EmailAddress] string email)
    {
        return Ok(new ProfileResponse());
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
    public IActionResult EditProfile(
        string email,
        [FromBody] EditProfileRequest request)
    {
        return NoContent();
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
    public IActionResult ResetPassword(string email)
    {
        return NoContent();
    }
}