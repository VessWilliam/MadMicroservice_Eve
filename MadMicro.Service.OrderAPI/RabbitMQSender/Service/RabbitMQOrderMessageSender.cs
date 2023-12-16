using MadMicro.Services.OrderAPI.RabbitMQSender.IService;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MadMicro.Services.OrderAPI.RabbitMQSender.Service;

public class RabbitMQOrderMessageSender : IRabbitMQOrderMessageSender
{

    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _passWord;
    private IConnection _connection;
    private const string OrderCreated_RewardsUpdateQueue = "RewardUpdateQueue";
    private const string OrderCreated_EmailUpdateQueue = "EmailUpdateQueue";
    private const string EmailRoutingKey = "EmailUpdateKey";
    private const string RewardRoutingKey = "RewardUpdateKey";

    public RabbitMQOrderMessageSender()
    {
        _hostName = "localhost";
        _userName = "guest";
        _passWord = "guest";
    }
    public void SendMessage(object message, string exchangeName)
    {
     
        if(ConnectionExist())
        {
            using var channel = _connection.CreateChannel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable:false);
            channel.QueueDeclare(OrderCreated_EmailUpdateQueue, false, false, false, null);
            channel.QueueDeclare(OrderCreated_RewardsUpdateQueue, false, false, false, null);

            channel.QueueBind(OrderCreated_EmailUpdateQueue, exchangeName, EmailRoutingKey);
            channel.QueueBind(OrderCreated_RewardsUpdateQueue, exchangeName, RewardRoutingKey);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublishAsync(exchange: exchangeName, routingKey: EmailRoutingKey, body: body);
            channel.BasicPublishAsync(exchange: exchangeName, routingKey: RewardRoutingKey, body: body);
        }
       
    }

    private void CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _passWord
            };

            _connection = factory.CreateConnection();

        }
        catch (Exception ex)
        {

            throw;
        }

    }

    private bool ConnectionExist()
    {
        if (_connection != null) return true;
        CreateConnection();
        return true;
    }


}
