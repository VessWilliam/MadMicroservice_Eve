using Azure.Messaging.ServiceBus;
using MadMicro.Services.EmailAPI.Models.DTO;
using MadMicro.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace MadMicro.Services.EmailAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string emailCartQueue;
    private readonly IConfiguration _config;

    private ServiceBusProcessor _emailCartprocessor;

    public AzureServiceBusConsumer(IConfiguration config)
    {
        _config = config;
        serviceBusConnectionString =
            _config.GetValue<string>("ServiceBusConnectionString") ?? string.Empty;

        emailCartQueue = _config.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue") ?? string.Empty;

        var clientBus = new ServiceBusClient(serviceBusConnectionString);
        _emailCartprocessor = clientBus.CreateProcessor(emailCartQueue);
    }

    public async Task Start()
    {
        _emailCartprocessor.ProcessMessageAsync += OnEmailCartRequestReceived;
        _emailCartprocessor.ProcessErrorAsync += ErrorHandler;
    }
    public async Task Stop()
    {
       await _emailCartprocessor.StopProcessingAsync();
       await _emailCartprocessor.DisposeAsync();
    }

    private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs msg)
    {
        //this is where will receive message
        var message = msg.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        CartDTO objMesage = JsonConvert.DeserializeObject<CartDTO>(body);

        try
        {
            //TODO - try to log message
            await msg.CompleteMessageAsync(msg.Message);
        }
        catch (Exception)
        {

            throw;
        }

    }

    private Task ErrorHandler(ProcessErrorEventArgs error)
    {
        Console.WriteLine(error.Exception.ToString());
        return Task.CompletedTask;
    }

   
}
