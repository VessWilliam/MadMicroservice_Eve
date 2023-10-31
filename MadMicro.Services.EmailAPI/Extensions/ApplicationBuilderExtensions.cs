using MadMicro.Services.EmailAPI.Services.IService;

namespace MadMicro.Services.EmailAPI.Extensions;

public static class ApplicationBuilderExtensions
{
    private static IAzureServiceBusConsumer serviceBusConsumer { get; set; }

    public static IApplicationBuilder UserAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        serviceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
        var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        hostApplicationLife.ApplicationStarted.Register(Start);
        hostApplicationLife.ApplicationStopping.Register(Stop);

        return app;
    }

    private static void Stop()
    {
        serviceBusConsumer?.Stop(); 
    }
    private static void Start()
    {
        serviceBusConsumer.Start();
    }

}
