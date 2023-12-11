namespace MadMicro.Services.ShoppingCartAPI.RabbitMQSender.IService;

public interface IRabbitMQShoppingCartMessageSender
{

    void SendMessage(Object message, string queuename);

}
