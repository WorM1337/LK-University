using Personal_Cabinet_Uni.Models.Entities;

namespace Personal_Cabinet_Uni.Services;

public interface INotificationPublisher
{
    Task PublishApplicantRegisteredAsync(Profile profile, CancellationToken cancellationToken = default);
    Task PublishLoginAsync(Profile profile, CancellationToken cancellationToken = default);
    Task PublishProfileUpdatedAsync(Profile profile, CancellationToken cancellationToken = default);
    Task PublishPasswordResetAsync(Profile profile, string temporaryPassword, CancellationToken cancellationToken = default);
    Task PublishManagerCreatedAsync(Profile profile, CancellationToken cancellationToken = default);
    Task PublishManagerUpdatedAsync(Profile profile, CancellationToken cancellationToken = default);
    Task PublishManagerDeletedAsync(Profile profile, CancellationToken cancellationToken = default);
}
