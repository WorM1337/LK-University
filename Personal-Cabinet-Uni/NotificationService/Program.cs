using MassTransit;
using NotificationService;
using NotificationService.Consumers;
using NotificationService.Workers;

var builder = Host.CreateApplicationBuilder(args);

// Configure Email Settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register Email Worker
builder.Services.AddScoped<IEmailWorker, EmailWorker>();

// Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<NotificationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqSettings = builder.Configuration.GetSection("RabbitMQ");
        
        cfg.Host(
            rabbitMqSettings["Host"] ?? "localhost",
            h =>
            {
                h.Username(rabbitMqSettings["Username"] ?? "guest");
                h.Password(rabbitMqSettings["Password"] ?? "guest");
            }
        );

        cfg.ReceiveEndpoint("notification-queue", e =>
        {
            e.ConfigureConsumer<NotificationConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
