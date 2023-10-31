namespace MadMicro.Services.EmailAPI.Services.IService;

public interface IAzureServiceBusConsumer
{
    Task Start();
    Task Stop();
}
