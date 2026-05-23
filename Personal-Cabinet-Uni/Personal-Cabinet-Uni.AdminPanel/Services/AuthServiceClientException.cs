namespace Personal_Cabinet_Uni.AdminPanel.Services;

public class AuthServiceClientException : Exception
{
    public int StatusCode { get; }

    public AuthServiceClientException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
