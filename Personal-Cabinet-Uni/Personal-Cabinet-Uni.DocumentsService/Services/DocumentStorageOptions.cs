namespace Personal_Cabinet_Uni.DocumentsService.Services;

public class DocumentStorageOptions
{
    public string RootPath { get; set; } = "storage/documents";
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;
}
