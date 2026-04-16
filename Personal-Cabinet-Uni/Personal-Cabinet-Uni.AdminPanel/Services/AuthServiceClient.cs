using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Personal_Cabinet_Uni.AdminPanel.Models.DTO.Request;
using Personal_Cabinet_Uni.Models.DTO.Response;

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
        var response = await _httpClient.PostAsJsonAsync("auth/login", request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken);
        }

        return null;
    }

    public async Task<ProfileResponse?> CreateManagerAsync(CreateManagerRequest request, string adminToken, CancellationToken cancellationToken = default)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _httpClient.PostAsJsonAsync("auth/manager", request, _jsonOptions, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProfileResponse>(_jsonOptions, cancellationToken);
        }

        return null;
    }

    public async Task<ProfileResponse?> EditManagerAsync(string email, EditManagerRequest request, string adminToken, CancellationToken cancellationToken = default)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _httpClient.PatchAsJsonAsync($"auth/manager/{email}", request, _jsonOptions, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProfileResponse>(_jsonOptions, cancellationToken);
        }

        return null;
    }

    public async Task<bool> DeleteManagerAsync(string email, string adminToken, CancellationToken cancellationToken = default)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _httpClient.DeleteAsync($"auth/manager/{email}", cancellationToken);

        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<ProfileResponse>?> GetAllManagersAsync(string adminToken, CancellationToken cancellationToken = default)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _httpClient.GetAsync("auth/managers", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<ProfileResponse>>(_jsonOptions, cancellationToken);
        }

        return null;
    }

    public async Task<ProfileResponse?> GetManagerByEmailAsync(string email, string adminToken, CancellationToken cancellationToken = default)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _httpClient.GetAsync($"auth/manager/{email}", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProfileResponse>(_jsonOptions, cancellationToken);
        }

        return null;
    }
}
