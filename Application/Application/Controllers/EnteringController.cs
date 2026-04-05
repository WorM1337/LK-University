using Application.Model.DTO.Request;
using Application.Model.DTO.Response;
using Application.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;
/// <summary>
/// Контроллер абитуриента (процесса поступления)
/// </summary>
[ApiController]
[Route("enter")]
public class EnteringController : ControllerBase
{
    /// <summary>
    /// Создание заявки на поступление
    /// </summary>
    /// <remarks>Определяет пользователя по JWT токену и находит его данные. Доступно только Абитуриенту</remarks>
    [HttpPost]
    [ProducesResponseType( 201)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [Authorize(Roles = nameof(Role.Appplicant))]
    public IActionResult SendApplication()
    {
        return NoContent();
    }
    /// <summary>
    /// Получение программ образования с пагинацией и фильтрацией
    /// </summary>
    /// <param name="page">Номер страницы</param>
    /// <param name="limit">Макс. количество объектов на странице</param>
    /// <param name="faculty">Факультет</param>
    /// <param name="educationLevel">Уровень образования</param>
    /// <param name="formOfEducation">Форма обучения</param>
    /// <param name="studyingLanguage">Язык обучения</param>
    /// <param name="name">Название программы(по частичному совпадению)</param>
    /// <param name="specialityCode">Код специальности</param>
    /// <returns>Список программ с факультетами и уровнем образования</returns>
    [HttpGet("program")]
    [ProducesResponseType(typeof(IEnumerable<ProgramResponse>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [Authorize]
    public IActionResult GetProgramsOfEducation(int page = 1, int limit = 20, string? faculty = null, string? educationLevel = null,
        FormOfEducation? formOfEducation = null, string? studyingLanguage = null,
        string? name = null, string? specialityCode = null)
    {
        return NoContent();
    }
    /// <summary>
    /// Получение программ абитуриента
    /// </summary>
    /// <returns>Список программ</returns>
    /// <remarks>Доступно только абитуриенту</remarks>
    [HttpGet("program/my")]
    [ProducesResponseType(typeof(IEnumerable<ProgramResponse>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [Authorize(Roles = nameof(Role.Appplicant))]
    public IActionResult GetMyPrograms()
    {
        return NoContent();
    }

    /// <summary>
    /// Добавление обр. программы в заявку на поступление по id программы
    /// </summary>
    /// <param name="id">ID программы</param>
    /// <returns>Добавленная программа</returns>
    /// <remarks>Доступно только абитуриенту</remarks>
    [HttpPost("program/{id}")]
    [ProducesResponseType(typeof(ProgramResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Authorize(Roles = nameof(Role.Appplicant))]
    public IActionResult AddProgramToApplication(Guid id)
    {
        return NoContent();
    }
    
    /// <summary>
    /// Удаление программы из заявки по id
    /// </summary>
    /// <param name="id">ID программы</param>
    /// <returns>Удалённая программа</returns>
    /// <remarks>Доступно только абитуриенту. Проверка заявки осуществляется на основе JWT</remarks>
    [HttpDelete("program/{id}")]
    [ProducesResponseType(typeof(ProgramResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Authorize(Roles = nameof(Role.Appplicant))]
    public IActionResult DeleteProgramFromApplication(Guid id)
    {
        return NoContent();
    }
    /// <summary>
    /// Изменение приоритета программ внутри заявки
    /// </summary>
    /// <param name="requests">Список запросов вида programId - приоритет</param>
    /// <returns>Список изменёных программ с приоритетами</returns>
    /// <remarks>Доступно только абитуриенту. Проверка заявки осуществляется на основе JWT</remarks>
    [HttpPatch("program")]
    [ProducesResponseType(typeof(IEnumerable<ProgramResponseWithPriority>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [Authorize(Roles = nameof(Role.Appplicant))]
    public IActionResult EditProgramPriority(IEnumerable<EditProgramPriorityRequest> requests)
    {
        return NoContent();
    }
    
}