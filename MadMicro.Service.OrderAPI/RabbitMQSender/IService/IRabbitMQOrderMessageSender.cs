namespace MadMicro.Services.OrderAPI.RabbitMQSender.IService;

public interface IRabbitMQOrderMessageSender
{

     Task SendMessage(object message, string exchangeName);
 
}
