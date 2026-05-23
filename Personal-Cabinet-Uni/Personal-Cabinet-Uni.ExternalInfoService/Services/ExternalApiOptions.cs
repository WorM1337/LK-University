namespace Personal_Cabinet_Uni.ExternalInfoService.Services;

public class ExternalApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int ProgramsPageSize { get; set; } = 100;
}
