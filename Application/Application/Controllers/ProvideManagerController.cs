using Application.Model.DTO.Request;
using Application.Model.DTO.Response;
using Application.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

/// <summary>
/// Контроллер для управления высшим персоналом (менеджерами и гл. менеджерами)
/// </summary>
[ApiController]
[Route("provideManager")]
public class ProvideManagerController : ControllerBase
{
    /// <summary>
    /// Регистрация (гл.) нового менеджера
    /// </summary>
    /// <param name="request">Запрос с данными менеджера</param>
    /// <remarks>Доступно только админу</remarks>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [Authorize(Roles = nameof(Role.Admin))]
    public IActionResult CreateManager(CreateManagerRequest request)
    {
        return NoContent();
    }

    /// <summary>
    /// Изменение профиля менеджера, гл. менеджера
    /// </summary>
    /// <param name="email">Адрес эл. почты</param>
    /// <param name="request">Запрос на изменение данных</param>
    /// <returns>Изменённый профиль</returns>
    /// <remarks>Доступно только админу</remarks>
    [HttpPatch("{email}")]
    [ProducesResponseType(typeof(ProfileResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Authorize(Roles = nameof(Role.Admin))]
    public IActionResult EditManager(string email, EditManagerRequest request)
    {
        return NoContent();
    }

    /// <summary>
    /// Удаление менеджера по email
    /// </summary>
    /// <param name="email">Адрес эл. почты</param>
    /// <returns>Удалённый профиль</returns>
    [HttpDelete("{email}")]
    [ProducesResponseType(typeof(ProfileResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Authorize(Roles = nameof(Role.Admin))]
    public IActionResult DeleteManager(string email)
    {
        return NoContent();
    }
}