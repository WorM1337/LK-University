using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Workers;
using Personal_Cabinet_Uni.Shared.Models.DTO.Request;

namespace NotificationService.Consumers;

public class NotificationConsumer : IConsumer<NotificationMessage>
{
    private readonly IEmailWorker _emailWorker;
    private readonly ILogger<NotificationConsumer> _logger;

    public NotificationConsumer(IEmailWorker emailWorker, ILogger<NotificationConsumer> logger)
    {
        _emailWorker = emailWorker;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NotificationMessage> context)
    {
        var message = context.Message;
        
        _logger.LogInformation("Processing notification for {To} with subject {Subject}", message.To, message.Subject);
        
        try
        {
            await _emailWorker.SendEmailAsync(message, context.CancellationToken);
            _logger.LogInformation("Notification sent successfully to {To}", message.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to {To}", message.To);
            throw; // Re-throw to trigger MassTransit retry mechanism
        }
    }
}
