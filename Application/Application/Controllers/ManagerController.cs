using Application.Model.DTO.Request;
using Application.Model.DTO.Response;
using Application.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;
/// <summary>
/// Контроллер по управлению поступлением абитуриента
/// </summary>
[ApiController]
[Route("manager")]
public class ManagerController : ControllerBase
{
    /// <summary>
    /// Получение всех заявок абитуриента
    /// </summary>
    /// <returns>Список заявок со статусами</returns>
    /// <remarks>Доступно всем, кроме абитуриента</remarks>
    [HttpGet("application")]
    [ProducesResponseType(typeof(IEnumerable<ApplicationResponse>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [Authorize(Roles = $"{nameof(Role.Manager)},{nameof(Role.MainManager)},{nameof(Role.Admin)}")]
    public IActionResult GetApplications()
    {
        return NoContent();
    }

    /// <summary>
    /// Получение подробной информации о конкретной заявке абитуриента 
    /// </summary>
    /// <param name="id">ID заявки абитуриента</param>
    /// <returns>Подробная информация о заявке</returns>
    [HttpGet("application/{id}")]
    [ProducesResponseType(typeof(DetailedApplicationResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Authorize]
    public IActionResult GetApplication(Guid id)
    {
        return NoContent();
    }

    /// <summary>
    /// Взятие заявки абитуриента в работу
    /// </summary>
    /// <param name="managerId">ID менеджера</param>
    /// <param name="applicationId">ID заявки, которую нужно взять</param>
    /// <returns>Взятая заявка</returns>
    /// <remarks>Доступно гл. менеджеру и админу</remarks>
    [HttpPost("program/take/{managerId}/{applicationId}")]
    [ProducesResponseType(typeof(DetailedApplicationResponse),200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Authorize(Roles = $"{nameof(Role.MainManager)},{nameof(Role.Admin)}")]
    public IActionResult TakeApplication(Guid managerId, Guid applicationId)
    {
        return NoContent();
    }
    /// <summary>
    /// Отказ от работы с заявкой абитуриента
    /// </summary>
    /// <param name="applicationId">ID заявки</param>
    /// <returns>Инфо о заявке, от которой отказались</returns>
    /// <remarks>Доступно всем, кроме абитуриента</remarks>
    [HttpPost("program/decline/{applicationId}")]
    [ProducesResponseType(typeof(DetailedApplicationResponse),200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Authorize(Roles = $"{nameof(Role.Manager)},{nameof(Role.MainManager)},{nameof(Role.Admin)}")]
    public IActionResult DeclineApplicationWork(Guid applicationId)
    {
        return NoContent();
    }
    
    /// <summary>
    /// Удаление программы из заявки
    /// </summary>
    /// <param name="applicationId">ID заявки</param>
    /// <param name="programId">ID программы</param>
    /// <returns>Удалённая программа</returns>
    /// <remarks>Доступно всем, кроме абитуриента</remarks>
    [HttpDelete("program/{applicationId}/{programId}")]
    [ProducesResponseType(typeof(ProgramResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Authorize(Roles = $"{nameof(Role.Manager)},{nameof(Role.MainManager)},{nameof(Role.Admin)}")]
    public IActionResult DeleteProgramFromApplication(Guid applicationId, Guid programId)
    {
        return NoContent();
    }
    /// <summary>
    /// Изменение приоритета программы у заданной заявки
    /// </summary>
    /// <param name="applicationId">ID заявки</param>
    /// <param name="requests">Список запросов вида programId - приоритет</param>
    /// <returns>Список изменённых программ</returns>
    [HttpPatch("program/{applicationId}")]
    [ProducesResponseType(typeof(IEnumerable<ProgramResponseWithPriority>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Authorize(Roles = $"{nameof(Role.Manager)},{nameof(Role.MainManager)},{nameof(Role.Admin)}")]
    public IActionResult EditProgramPriority(Guid applicationId, IEnumerable<EditProgramPriorityRequest> requests)
    {
        return NoContent();
    }

    /// <summary>
    /// Изменение статуса поступления у заявки
    /// </summary>
    /// <param name="applicationId">ID заявки</param>
    /// <param name="status">Новый статус заявки</param>
    /// <returns>Изменённая заявка</returns>
    [HttpPatch("enteringStatus/{applicationId}")]
    [ProducesResponseType(typeof(ApplicationResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Authorize(Roles = $"{nameof(Role.Manager)},{nameof(Role.MainManager)},{nameof(Role.Admin)}")]
    public IActionResult EditEnteringStatus(Guid applicationId, EnteringStatus status)
    {
        return NoContent();
    }
}