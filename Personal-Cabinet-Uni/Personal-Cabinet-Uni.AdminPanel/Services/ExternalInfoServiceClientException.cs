namespace Personal_Cabinet_Uni.AdminPanel.Services;

public class ExternalInfoServiceClientException : Exception
{
    public int StatusCode { get; }

    public ExternalInfoServiceClientException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
