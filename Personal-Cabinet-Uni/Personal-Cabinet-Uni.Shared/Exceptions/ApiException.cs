namespace Personal_Cabinet_Uni.Shared.Exceptions;

/// <summary>
/// Базовое исключение для API с кодом статуса HTTP
/// </summary>
public abstract class ApiException : Exception
{
    /// <summary>
    /// HTTP статус код ошибки
    /// </summary>
    public int StatusCode { get; }

    protected ApiException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    protected ApiException(string message, int statusCode, Exception innerException) : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
