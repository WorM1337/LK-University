namespace Application.Model.DTO.Response;

/// <summary>
/// Ответ на регистрацию\логин
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// JWT токен
    /// </summary>
    public string Token { get; set; }
}