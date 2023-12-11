using RabbitMQ.Client;
using MadMicro.Services.EmailAPI.Services.Service;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using MadMicro.Services.EmailAPI.Models.DTO;


namespace MadMicro.Services.EmailAPI.Messaging;

public class RabbitMQShoppingCartConsumer : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly EmailService _emailService;
    private readonly string registerUserQueue;
    private IConnection _connection;
    private IChannel _channel;
    public RabbitMQShoppingCartConsumer(IConfiguration config, EmailService emailService)
    {
        _config = config;
        _emailService = emailService;
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateChannel();
        registerUserQueue = _config.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue")!;
        _channel.QueueDeclare(registerUserQueue, false, false, false, null);
    }

    protected override async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
    {
       stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(channel: _channel);
        consumer.Received += async (c, e) =>
        {

            var content =  Encoding.UTF8.GetString(e.Body.ToArray());
            var cartDto = JsonConvert.DeserializeObject<CartDTO>(content);
            await HandleMessage(cartDto);

            _channel.BasicAck(e.DeliveryTag, false);
        };

        await _channel.BasicConsumeAsync(registerUserQueue,false,consumer);
        return Task.CompletedTask;  
    }

    private async Task HandleMessage(CartDTO cartDTO)
    {
       await _emailService.EmailCartAndLog(cartDTO);
    }

   
}
