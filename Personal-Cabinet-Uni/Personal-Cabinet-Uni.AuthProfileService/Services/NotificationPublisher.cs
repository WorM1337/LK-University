using MassTransit;
using Personal_Cabinet_Uni.Models.Entities;
using Personal_Cabinet_Uni.Shared.Models.DTO.Request;

namespace Personal_Cabinet_Uni.Services;

public class NotificationPublisher : INotificationPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<NotificationPublisher> _logger;

    public NotificationPublisher(IPublishEndpoint publishEndpoint, ILogger<NotificationPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public Task PublishApplicantRegisteredAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        return PublishAsync(
            profile.Email,
            "Регистрация в личном кабинете абитуриента",
            $"Здравствуйте, {GetDisplayName(profile)}! Ваша учетная запись успешно создана.",
            cancellationToken);
    }

    public Task PublishLoginAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        return PublishAsync(
            profile.Email,
            "Вход в личный кабинет",
            $"Здравствуйте, {GetDisplayName(profile)}! Выполнен вход в вашу учетную запись.",
            cancellationToken);
    }

    public Task PublishProfileUpdatedAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        return PublishAsync(
            profile.Email,
            "Профиль обновлен",
            $"Здравствуйте, {GetDisplayName(profile)}! Данные вашего профиля были обновлены.",
            cancellationToken);
    }

    public Task PublishPasswordResetAsync(Profile profile, string temporaryPassword, CancellationToken cancellationToken = default)
    {
        return PublishAsync(
            profile.Email,
            "Сброс пароля",
            $"Здравствуйте, {GetDisplayName(profile)}! Ваш временный пароль: {temporaryPassword}",
            cancellationToken);
    }

    public Task PublishManagerCreatedAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        return PublishAsync(
            profile.Email,
            "Учетная запись сотрудника создана",
            $"Здравствуйте, {GetDisplayName(profile)}! Для вас создана учетная запись с ролью {profile.Role}.",
            cancellationToken);
    }

    public Task PublishManagerUpdatedAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        return PublishAsync(
            profile.Email,
            "Данные учетной записи сотрудника обновлены",
            $"Здравствуйте, {GetDisplayName(profile)}! Данные вашей учетной записи были обновлены.",
            cancellationToken);
    }

    public Task PublishManagerDeletedAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        return PublishAsync(
            profile.Email,
            "Учетная запись сотрудника удалена",
            $"Здравствуйте, {GetDisplayName(profile)}! Ваша учетная запись сотрудника была удалена.",
            cancellationToken);
    }

    private async Task PublishAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            _logger.LogWarning("Notification was not published because recipient email is empty");
            return;
        }

        try
        {
            await _publishEndpoint.Publish(new NotificationMessage
            {
                To = to,
                Subject = subject,
                Body = body,
                IsHtml = false
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish notification for {To} with subject {Subject}", to, subject);
        }
    }

    private static string GetDisplayName(Profile profile)
    {
        var fullName = $"{profile.Surname} {profile.Name}".Trim();
        return string.IsNullOrWhiteSpace(fullName) ? profile.Email : fullName;
    }
}
