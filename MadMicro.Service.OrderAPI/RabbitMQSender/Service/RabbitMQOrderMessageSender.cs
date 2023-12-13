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
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable:false);
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublishAsync(exchange: exchangeName, routingKey: string.Empty, body: body);
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
