using System.ComponentModel.DataAnnotations;
using Application.Model.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

/// <summary>
/// Контроллер документов
/// </summary>
[ApiController]
[Route("document")]
public class DocumentController : ControllerBase
{
    /// <summary>
    /// Загрузка документа (бинарные данные)
    /// </summary>
    /// <param name="file">Файл</param>
    /// <returns>Объект с URL файла</returns>
    [HttpPost]
    [ProducesResponseType(typeof(DocumentResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [Authorize]
    public IActionResult UploadDocument(IFormFile file)
    {
        return NoContent();
    }

    /// <summary>
    /// Получение файла по URL
    /// </summary>
    /// <param name="url">URL файла</param>
    /// <returns>Бинарные данные</returns>
    [HttpGet("{url}")]
    [ProducesResponseType(typeof(FileResult), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [Authorize]
    public IActionResult GetDocument(string url)
    {
        return NoContent();
    }
    
    
    /// <summary>
    /// Удаление файла по URL
    /// </summary>
    /// <param name="url">URL файла</param>
    /// <returns>URL удалённого файла и его название</returns>
    [HttpDelete("{url}")]
    [ProducesResponseType(typeof(DocumentResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Authorize]
    public IActionResult DeleteDocument(string url)
    {
        return NoContent();
    }
    
    /// <summary>
    /// Редактирование имени файла
    /// </summary>
    /// <param name="url">URL файла</param>
    /// <param name="name">Новое имя файла</param>
    /// <returns>URL и изменённое имя</returns>
    [HttpPatch("name/{url}")]
    [ProducesResponseType(typeof(DocumentResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Authorize]
    public IActionResult EditDocumentName(string url, [FromQuery] [Required] string name)
    {
        return NoContent();
    }
}