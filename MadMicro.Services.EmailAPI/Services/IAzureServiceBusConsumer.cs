namespace MadMicro.Services.EmailAPI.Services;

public interface IAzureServiceBusConsumer
{
    Task Start();
    Task Stop();
}
