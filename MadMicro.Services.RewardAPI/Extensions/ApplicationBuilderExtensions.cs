using MadMicro.Services.RewardAPI.Services.IService;

namespace MadMicro.Services.RewardAPI.Extensions;

public static class ApplicationBuilderExtensions
{
    private static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }

    public static IApplicationBuilder UserAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
        var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        hostApplicationLife.ApplicationStarted.Register(Start);
        hostApplicationLife.ApplicationStopping.Register(Stop);

        return app;
    }

    private static void Stop()
    {
        ServiceBusConsumer?.Stop(); 
    }
    private static void Start()
    {
        ServiceBusConsumer.Start();
    }

}
