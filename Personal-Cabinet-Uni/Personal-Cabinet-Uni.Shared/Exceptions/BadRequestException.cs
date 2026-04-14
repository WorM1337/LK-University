namespace Personal_Cabinet_Uni.Shared.Exceptions;

/// <summary>
/// Исключение для некорректных данных запроса (400 Bad Request)
/// </summary>
public class BadRequestException : ApiException
{
    public BadRequestException(string message) : base(message, 400)
    {
    }

    public BadRequestException(string message, Exception innerException) : base(message, 400, innerException)
    {
    }
}
