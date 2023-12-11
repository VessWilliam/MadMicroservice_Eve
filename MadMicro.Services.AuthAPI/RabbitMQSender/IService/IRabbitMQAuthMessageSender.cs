namespace MadMicro.Services.AuthAPI.RabbitMQSender.IService;

public interface IRabbitMQAuthMessageSender
{

    void SendMessage(Object message, string queuename);

}
