using Application.Model.DTO.Response;
using Application.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;
/// <summary>
/// Контроллер внешней информации
/// </summary>
[ApiController]
[Route("externalInfo")]
public class ExternalInfoController : ControllerBase
{
    /// <summary>
    /// Импорт акт. уровней образования
    /// </summary>
    [HttpPost("educationLevels")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse),401)]
    [ProducesResponseType(typeof(ErrorResponse),403)]
    [Authorize(Roles = nameof(Role.Admin))]
    public IActionResult GetEducationLevels()
    {
        return NoContent();
    }
    
    /// <summary>
    /// Получение статуса импорта уровней образования
    /// </summary>
    /// <returns>Статус</returns>
    [HttpGet("educationLevels/status")]
    [ProducesResponseType(typeof(ImportingStatusResponse),200)]
    [ProducesResponseType(typeof(ErrorResponse),401)]
    [ProducesResponseType(typeof(ErrorResponse),403)]
    [Authorize(Roles = nameof(Role.Admin))]
    public IActionResult GetEducationLevelsStatus()
    {
        return NoContent();
    }
    /// <summary>
    /// Импорт акт. типов документов
    /// </summary>
    [HttpPost("documentTypes")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse),401)]
    [ProducesResponseType(typeof(ErrorResponse),403)]
    [Authorize(Roles = nameof(Role.Admin))]
    public IActionResult GetDocumentTypes()
    {
        return NoContent();
    }
    /// <summary>
    /// Получение статуса импорта типов документов
    /// </summary>
    /// <returns>Статус</returns>
    [HttpGet("documentTypes/status")]
    [ProducesResponseType(typeof(ImportingStatusResponse),200)]
    [ProducesResponseType(typeof(ErrorResponse),401)]
    [ProducesResponseType(typeof(ErrorResponse),403)]
    [Authorize(Roles = nameof(Role.Admin))]
    public IActionResult GetDocumentTypesStatus()
    {
        return NoContent();
    }
    /// <summary>
    /// Импорт акт. факультетов
    /// </summary>
    [HttpPost("faculties")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse),401)]
    [ProducesResponseType(typeof(ErrorResponse),403)]
    [Authorize(Roles = nameof(Role.Admin))]
    public IActionResult GetFaculties()
    {
        return NoContent();
    }
    /// <summary>
    /// Получение статуса импорта факультетов
    /// </summary>
    /// <returns>Статус</returns>
    [HttpGet("faculties/status")]
    [ProducesResponseType(typeof(ImportingStatusResponse),200)]
    [ProducesResponseType(typeof(ErrorResponse),401)]
    [ProducesResponseType(typeof(ErrorResponse),403)]
    [Authorize(Roles = nameof(Role.Admin))]
    public IActionResult GetFacultiesStatus()
    {
        return NoContent();
    }
    /// <summary>
    /// Импорт акт. программ
    /// </summary>
    [HttpPost("programs")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse),401)]
    [ProducesResponseType(typeof(ErrorResponse),403)]
    [Authorize(Roles = nameof(Role.Admin))]
    public IActionResult GetPrograms()
    {
        return NoContent();
    }
    /// <summary>
    /// Получение статуса импорта программ
    /// </summary>
    /// <returns>Статус</returns>
    [HttpGet("programs/status")]
    [ProducesResponseType(typeof(ImportingStatusResponse),200)]
    [ProducesResponseType(typeof(ErrorResponse),401)]
    [ProducesResponseType(typeof(ErrorResponse),403)]
    [Authorize(Roles = nameof(Role.Admin))]
    public IActionResult GetProgramsStatus()
    {
        return NoContent();
    }
}