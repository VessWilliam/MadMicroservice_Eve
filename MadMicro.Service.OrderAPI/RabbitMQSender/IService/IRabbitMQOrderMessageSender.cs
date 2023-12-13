namespace MadMicro.Services.OrderAPI.RabbitMQSender.IService;

public interface IRabbitMQOrderMessageSender
{

     void SendMessage(object message, string exchangeName);
 
}
