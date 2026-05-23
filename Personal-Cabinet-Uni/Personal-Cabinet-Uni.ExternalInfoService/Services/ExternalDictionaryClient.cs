using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Personal_Cabinet_Uni.ExternalInfoService.Models.DTO.External;

namespace Personal_Cabinet_Uni.ExternalInfoService.Services;

public class ExternalDictionaryClient : IExternalDictionaryClient
{
    private readonly HttpClient _httpClient;
    private readonly ExternalApiOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExternalDictionaryClient(HttpClient httpClient, IOptions<ExternalApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.Username}:{_options.Password}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
    }

    public async Task<List<ExternalEducationLevelDto>> GetEducationLevelsAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<ExternalEducationLevelDto>>("api/dictionary/education_levels", cancellationToken) ?? [];
    }

    public async Task<List<ExternalEducationDocumentTypeDto>> GetDocumentTypesAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<ExternalEducationDocumentTypeDto>>("api/dictionary/document_types", cancellationToken) ?? [];
    }

    public async Task<List<ExternalFacultyDto>> GetFacultiesAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<ExternalFacultyDto>>("api/dictionary/faculties", cancellationToken) ?? [];
    }

    public async Task<List<ExternalEducationProgramDto>> GetProgramsAsync(CancellationToken cancellationToken = default)
    {
        var programs = new List<ExternalEducationProgramDto>();
        var page = 1;
        var size = Math.Clamp(_options.ProgramsPageSize, 1, 500);

        while (true)
        {
            var response = await GetAsync<ExternalProgramPagedListDto>($"api/dictionary/programs?page={page}&size={size}", cancellationToken);
            if (response?.Programs is { Count: > 0 })
            {
                programs.AddRange(response.Programs);
            }

            if (response?.Pagination == null || page >= response.Pagination.Count || response.Programs.Count == 0)
            {
                break;
            }

            page++;
        }

        return programs;
    }

    private async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken);
    }
}
