using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Personal_Cabinet_Uni.AdminPanel.Models.DTO.Request;
using Personal_Cabinet_Uni.AdminPanel.Models.DTO.Response;
using Personal_Cabinet_Uni.Shared.Models.DTO.Response;

namespace Personal_Cabinet_Uni.AdminPanel.Services;

public class AuthServiceClient : IAuthServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync("auth/login", request, _jsonOptions, cancellationToken);
        return await ReadOrThrowAsync<AuthResponse>(response, cancellationToken);
    }

    public async Task<ManagerResponse?> CreateManagerAsync(CreateManagerRequest request, string adminToken, CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateAuthorizedRequest(HttpMethod.Post, "auth/manager", adminToken);
        httpRequest.Content = JsonContent.Create(request, options: _jsonOptions);
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        return await ReadOrThrowAsync<ManagerResponse>(response, cancellationToken);
    }

    public async Task<ManagerResponse?> EditManagerAsync(string email, EditManagerRequest request, string adminToken, CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateAuthorizedRequest(HttpMethod.Patch, $"auth/manager/{Uri.EscapeDataString(email)}", adminToken);
        httpRequest.Content = JsonContent.Create(request, options: _jsonOptions);
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        return await ReadOrThrowAsync<ManagerResponse>(response, cancellationToken);
    }

    public async Task<bool> DeleteManagerAsync(string email, string adminToken, CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateAuthorizedRequest(HttpMethod.Delete, $"auth/manager/{Uri.EscapeDataString(email)}", adminToken);
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return true;
    }

    public async Task<IEnumerable<ManagerResponse>?> GetAllManagersAsync(string adminToken, CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateAuthorizedRequest(HttpMethod.Get, "auth/managers", adminToken);
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        return await ReadOrThrowAsync<IEnumerable<ManagerResponse>>(response, cancellationToken);
    }

    public async Task<ManagerResponse?> GetManagerByEmailAsync(string email, string adminToken, CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateAuthorizedRequest(HttpMethod.Get, $"auth/manager/{Uri.EscapeDataString(email)}", adminToken);
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        return await ReadOrThrowAsync<ManagerResponse>(response, cancellationToken);
    }

    private static HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string requestUri, string token)
    {
        var request = new HttpRequestMessage(method, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private async Task<T?> ReadOrThrowAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken);
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = await ReadErrorMessageAsync(response, cancellationToken);
        throw new AuthServiceClientException((int)response.StatusCode, message);
    }

    private async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(_jsonOptions, cancellationToken);
            if (!string.IsNullOrWhiteSpace(error?.Message))
            {
                return error.Message;
            }
        }
        catch (JsonException)
        {
        }

        return $"Auth service returned {(int)response.StatusCode} {response.ReasonPhrase}";
    }
}
