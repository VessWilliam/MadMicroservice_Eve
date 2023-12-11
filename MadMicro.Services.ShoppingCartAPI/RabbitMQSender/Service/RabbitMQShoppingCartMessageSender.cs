using MadMicro.Services.ShoppingCartAPI.RabbitMQSender.IService;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MadMicro.Services.ShoppingCartAPI.RabbitMQSender.Service;

public class RabbitMQShoppingCartMessageSender : IRabbitMQShoppingCartMessageSender
{

    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _passWord;
    private IConnection _connection;

    public RabbitMQShoppingCartMessageSender()
    {
        _hostName = "localhost";
        _userName = "guest";
        _passWord = "guest";
    }
    public void SendMessage(object message, string queuename)
    {
     
        if(ConnectionExist())
        {
            using var channel = _connection.CreateChannel();
            channel.QueueDeclare(queuename, false, false, false, null);
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: string.Empty, routingKey: queuename, body: body);
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
