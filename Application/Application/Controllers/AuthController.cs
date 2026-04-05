using Application.Model.DTO.Request;
using Application.Model.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Register of user
    /// </summary>
    /// <param name="request">Данные на регистрацию</param>
    /// <returns>Объект с JWT токеном</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        return NoContent();
    }
    /// <summary>
    /// Логин пользователя
    /// </summary>
    /// <param name="request">Логин и пароль</param>
    /// <returns>Объект с JWT токеном</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        return NoContent();
    }
}