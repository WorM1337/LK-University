namespace Personal_Cabinet_Uni.Shared.Exceptions;

/// <summary>
/// Исключение для доступа запрещен (403 Forbidden)
/// </summary>
public class ForbiddenException : ApiException
{
    public ForbiddenException(string message) : base(message, 403)
    {
    }

    public ForbiddenException(string message, Exception innerException) : base(message, 403, innerException)
    {
    }
}
