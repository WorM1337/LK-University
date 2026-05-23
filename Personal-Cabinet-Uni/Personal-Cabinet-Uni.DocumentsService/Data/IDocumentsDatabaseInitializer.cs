namespace Personal_Cabinet_Uni.DocumentsService.Data;

public interface IDocumentsDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
