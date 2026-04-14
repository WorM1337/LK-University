namespace Personal_Cabinet_Uni.Shared.Exceptions;

/// <summary>
/// Исключение для неавторизованного доступа (401 Unauthorized)
/// </summary>
public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message) : base(message, 401)
    {
    }

    public UnauthorizedException(string message, Exception innerException) : base(message, 401, innerException)
    {
    }
}
