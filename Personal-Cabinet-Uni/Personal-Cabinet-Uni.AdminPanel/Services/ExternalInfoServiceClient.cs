using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Personal_Cabinet_Uni.AdminPanel.Models.DTO.ExternalInfo;
using Personal_Cabinet_Uni.Shared.Models.DTO.Response;

namespace Personal_Cabinet_Uni.AdminPanel.Services;

public class ExternalInfoServiceClient : IExternalInfoServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExternalInfoServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IReadOnlyCollection<ImportStatusResponse>> GetStatusesAsync(string adminToken, CancellationToken cancellationToken = default)
    {
        var endpoints = new[]
        {
            "externalInfo/educationLevels/status",
            "externalInfo/documentTypes/status",
            "externalInfo/faculties/status",
            "externalInfo/programs/status"
        };

        var statuses = new List<ImportStatusResponse>();
        foreach (var endpoint in endpoints)
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Get, endpoint, adminToken);
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var status = await ReadOrThrowAsync<ImportStatusResponse>(response, cancellationToken);
            if (status != null)
            {
                statuses.Add(status);
            }
        }

        return statuses;
    }

    public Task<ImportStatusResponse?> ImportEducationLevelsAsync(string adminToken, CancellationToken cancellationToken = default)
    {
        return PostImportAsync("externalInfo/educationLevels", adminToken, cancellationToken);
    }

    public Task<ImportStatusResponse?> ImportDocumentTypesAsync(string adminToken, CancellationToken cancellationToken = default)
    {
        return PostImportAsync("externalInfo/documentTypes", adminToken, cancellationToken);
    }

    public Task<ImportStatusResponse?> ImportFacultiesAsync(string adminToken, CancellationToken cancellationToken = default)
    {
        return PostImportAsync("externalInfo/faculties", adminToken, cancellationToken);
    }

    public Task<ImportStatusResponse?> ImportProgramsAsync(string adminToken, CancellationToken cancellationToken = default)
    {
        return PostImportAsync("externalInfo/programs", adminToken, cancellationToken);
    }

    private async Task<ImportStatusResponse?> PostImportAsync(string endpoint, string adminToken, CancellationToken cancellationToken)
    {
        using var request = CreateAuthorizedRequest(HttpMethod.Post, endpoint, adminToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        return await ReadOrThrowAsync<ImportStatusResponse>(response, cancellationToken);
    }

    private static HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string requestUri, string token)
    {
        var request = new HttpRequestMessage(method, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private async Task<T?> ReadOrThrowAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken);
        }

        var message = await ReadErrorMessageAsync(response, cancellationToken);
        throw new ExternalInfoServiceClientException((int)response.StatusCode, message);
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

        return $"External info service returned {(int)response.StatusCode} {response.ReasonPhrase}";
    }
}
