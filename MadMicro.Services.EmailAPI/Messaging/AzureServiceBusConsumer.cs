using Azure.Messaging.ServiceBus;
using MadMicro.Services.EmailAPI.Models.DTO;
using MadMicro.Services.EmailAPI.Services.IService;
using MadMicro.Services.EmailAPI.Services.Service;
using Newtonsoft.Json;
using System.Text;

namespace MadMicro.Services.EmailAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string emailCartQueue;
    private readonly string registerUserQueue;
    private readonly IConfiguration _config;
    private readonly EmailService _emailService;   

    private ServiceBusProcessor _emailCartprocessor;
    private ServiceBusProcessor _registerUserprocessor;

    public AzureServiceBusConsumer(IConfiguration config, EmailService emailService)
    {
        _config = config;
        serviceBusConnectionString =
            _config.GetValue<string>("ServiceBusConnectionString") ?? string.Empty;

        emailCartQueue = _config.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue") ?? string.Empty;
        registerUserQueue = _config.GetValue<string>("TopicAndQueueNames:RegisterUserQueue") ?? string.Empty;

        var clientBus = new ServiceBusClient(serviceBusConnectionString);
        _emailCartprocessor = clientBus.CreateProcessor(emailCartQueue);
        _registerUserprocessor = clientBus.CreateProcessor(registerUserQueue);
        _emailService = emailService;   
    }
    public async Task Start()
    {
        _emailCartprocessor.ProcessMessageAsync += OnEmailCartRequestReceived;
        _emailCartprocessor.ProcessErrorAsync += ErrorHandler;
        await _emailCartprocessor.StartProcessingAsync();
        
        _registerUserprocessor.ProcessMessageAsync += OnUserRegisterRequestReceived;
        _registerUserprocessor.ProcessErrorAsync += ErrorHandler;
        await _registerUserprocessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
       await _emailCartprocessor.StopProcessingAsync();
       await _emailCartprocessor.DisposeAsync();

       await _registerUserprocessor.StopProcessingAsync();
       await _registerUserprocessor.DisposeAsync();
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
            await _emailService.EmailCartAndLog(objMesage);
            await msg.CompleteMessageAsync(msg.Message);
        }
        catch (Exception)
        {
            throw;
        }

    }

    private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs msg)
    {
        //this is where will receive message
        var message = msg.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        var objMesage = JsonConvert.DeserializeObject<string>(body);

        try
        {
            //TODO - try to log message
            await _emailService.RegisterUserEmailAndLog(objMesage);
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
