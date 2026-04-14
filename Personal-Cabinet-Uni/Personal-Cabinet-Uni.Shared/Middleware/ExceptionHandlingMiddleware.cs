using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Personal_Cabinet_Uni.Shared.Exceptions;
using Personal_Cabinet_Uni.Shared.Models.DTO.Response;

namespace Personal_Cabinet_Uni.Shared.Middleware;

/// <summary>
/// Глобальный обработчик исключений для API
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case ApiException apiException:
                response.StatusCode = apiException.StatusCode;
                errorResponse.Status = apiException.StatusCode;
                errorResponse.Message = apiException.Message;
                _logger.LogWarning(apiException, "API Exception: {Message}", apiException.Message);
                break;

            case UnauthorizedAccessException _:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Status = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Доступ запрещен. Требуется аутентификация.";
                _logger.LogWarning(exception, "Unauthorized Access");
                break;

            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Status = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = argEx.Message;
                _logger.LogWarning(exception, "Bad Request: {Message}", argEx.Message);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Status = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "Произошла внутренняя ошибка сервера";
                _logger.LogError(exception, "Internal Server Error");
                break;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(errorResponse, options);
        await response.WriteAsync(json);
    }
}
