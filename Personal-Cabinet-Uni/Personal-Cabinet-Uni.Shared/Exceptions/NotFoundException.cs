namespace Personal_Cabinet_Uni.Shared.Exceptions;

/// <summary>
/// Исключение для ресурса, который не был найден (404 Not Found)
/// </summary>
public class NotFoundException : ApiException
{
    public NotFoundException(string message) : base(message, 404)
    {
    }

    public NotFoundException(string message, Exception innerException) : base(message, 404, innerException)
    {
    }
}
