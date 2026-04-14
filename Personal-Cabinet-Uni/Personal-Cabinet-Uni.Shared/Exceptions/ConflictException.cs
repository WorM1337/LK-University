namespace Personal_Cabinet_Uni.Shared.Exceptions;

/// <summary>
/// Исключение для конфликта данных (409 Conflict)
/// </summary>
public class ConflictException : ApiException
{
    public ConflictException(string message) : base(message, 409)
    {
    }

    public ConflictException(string message, Exception innerException) : base(message, 409, innerException)
    {
    }
}
