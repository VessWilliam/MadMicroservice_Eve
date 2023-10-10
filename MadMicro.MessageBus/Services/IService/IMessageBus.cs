namespace MadMicro.MessageBus;

public interface IMessageBus
{

    Task PublishMessage(object message, string topic_queueName);
}
