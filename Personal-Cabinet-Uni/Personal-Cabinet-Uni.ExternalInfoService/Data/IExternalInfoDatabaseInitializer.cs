namespace Personal_Cabinet_Uni.ExternalInfoService.Data;

public interface IExternalInfoDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
