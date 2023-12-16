using System.Text;
using MadMicro.Services.RewardAPI.Message;
using MadMicro.Services.RewardAPI.Services.Service;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace MadMicro.Services.RewardAPI.Messaging;

public class RabbitMQOrdersConsumer : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly RewardsService _rewardsService;
    private readonly string OrdersQueue;
    private IConnection _connection;
    private IChannel _channel;

    private const string OrderCreated_RewardsUpdateQueue = "RewardUpdateQueue";
    private const string RewardRoutingKey = "RewardUpdateKey";
    private readonly string ExchangeName = string.Empty;

    public RabbitMQOrdersConsumer(IConfiguration config, RewardsService rewardsService)
    {
        _config = config;
        _rewardsService = rewardsService;
        ExchangeName = _config.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic")!;
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateChannel();
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, false, false, null);

        _channel.QueueDeclare(OrderCreated_RewardsUpdateQueue,false,false,false,null);
        _channel.QueueBind(OrderCreated_RewardsUpdateQueue, ExchangeName, RewardRoutingKey);
    }

    protected override async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
    {
       stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(channel: _channel);
        consumer.Received += async (c, e) =>
        {

            var content =  Encoding.UTF8.GetString(e.Body.ToArray());
            var rewardMessage = JsonConvert.DeserializeObject<RewardMessage>(content);
            HandleMessage(rewardMessage).GetAwaiter().GetResult();

            _channel.BasicAck(e.DeliveryTag, false);
        };

        _channel.BasicConsume(OrderCreated_RewardsUpdateQueue,false,consumer);
        return Task.CompletedTask;  
    }

    private async Task HandleMessage(RewardMessage rewardMessage)
    {
        _rewardsService.UpdateRewards(rewardMessage).GetAwaiter().GetResult();
    }

   
}
