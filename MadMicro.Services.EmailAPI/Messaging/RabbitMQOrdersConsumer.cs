using RabbitMQ.Client;
using MadMicro.Services.EmailAPI.Services.Service;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using MadMicro.Services.EmailAPI.Models;
using MadMicro.Services.EmailAPI.Message;


namespace MadMicro.Services.EmailAPI.Messaging;

public class RabbitMQOrdersConsumer : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly EmailService _emailService;
    private readonly string OrdersQueue;
    private IConnection _connection;
    private IChannel _channel;
    private readonly string queueName = string.Empty;

    private const string OrderCreated_EmailUpdateQueue = "EmailUpdateQueue";
    private const string EmailRoutingKey = "EmailUpdateKey";
    private readonly string ExchangeName = string.Empty;



    public RabbitMQOrdersConsumer(IConfiguration config, EmailService emailService)
    {
        _config = config;
        _emailService = emailService;
        ExchangeName = _config.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic")!;
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateChannel();

        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(OrderCreated_EmailUpdateQueue,false,false,false,null);
        _channel.QueueBind(OrderCreated_EmailUpdateQueue,ExchangeName,EmailRoutingKey);
    }

    protected override async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
    {
       stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(channel: _channel);
        consumer.Received += async (c, e) =>
        {

            var content =  Encoding.UTF8.GetString(e.Body.ToArray());
            var rewardMessage = JsonConvert.DeserializeObject<OrderConfirmation>(content);
            HandleMessage(rewardMessage).GetAwaiter().GetResult();

            _channel.BasicAck(e.DeliveryTag, false);
        };

        _channel.BasicConsume(OrderCreated_EmailUpdateQueue,false,consumer);
        return Task.CompletedTask;  
    }

    private async Task HandleMessage(OrderConfirmation rewardMessage)
    {
       _emailService.LogOrderPlaced(rewardMessage).GetAwaiter().GetResult();
    }

   
}
