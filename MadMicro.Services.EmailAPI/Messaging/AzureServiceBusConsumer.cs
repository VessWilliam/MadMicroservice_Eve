using Azure.Messaging.ServiceBus;
using MadMicro.Services.EmailAPI.Message;
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
    private readonly string orderCreated_Topic;
    private readonly string orderCreated_Email_Subcription;

    private ServiceBusProcessor _emailOrderPlacedProcessor;
    private ServiceBusProcessor _emailCartprocessor;
    private ServiceBusProcessor _registerUserprocessor;

    public AzureServiceBusConsumer(IConfiguration config, EmailService emailService)
    {
        _config = config;
        serviceBusConnectionString = _config.GetValue<string>("ServiceBusConnectionString")!;

        emailCartQueue = _config.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue")!;
        registerUserQueue = _config.GetValue<string>("TopicAndQueueNames:RegisterUserQueue")!;

        orderCreated_Topic = _config.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic")!;
        orderCreated_Email_Subcription = _config.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription")!;


        var clientBus = new ServiceBusClient(serviceBusConnectionString);
        _emailCartprocessor = clientBus.CreateProcessor(emailCartQueue);
        _registerUserprocessor = clientBus.CreateProcessor(registerUserQueue);
        _emailOrderPlacedProcessor = clientBus.CreateProcessor(orderCreated_Topic,orderCreated_Email_Subcription);
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

        _emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequestReceived;
        _emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;
        await _emailOrderPlacedProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
       await _emailCartprocessor.StopProcessingAsync();
       await _emailCartprocessor.DisposeAsync();

       await _registerUserprocessor.StopProcessingAsync();
       await _registerUserprocessor.DisposeAsync();
        
       await _emailOrderPlacedProcessor.StopProcessingAsync();
       await _emailOrderPlacedProcessor.DisposeAsync();
    }


    private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs msg)
    {
        var message = msg.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        var objMessage = JsonConvert.DeserializeObject<OrderConfirmation>(body);
        try
        {
            await _emailService.LogOrderPlaced(objMessage);
            await msg.CompleteMessageAsync(msg.Message);

        }
        catch (Exception)
        {

            throw;
        }

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
