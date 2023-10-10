using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace MadMicro.MessageBus.Services.Service;

public class MessageBus : IMessageBus
{
    private readonly string connectionString = "Endpoint=sb://madmicroweb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=CXFo/+naeY8SHE5xZBIZvQXtQNVwmYghE+ASbJhHYcE=";
    public async Task PublishMessage(object message, string topic_queueName)
    {
        await using var client = new ServiceBusClient(connectionString);
        var sender = client.CreateSender(topic_queueName);
        var jsonMessage = JsonConvert.SerializeObject(message);

        var finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString(),
        };

        await sender.SendMessageAsync(finalMessage);    
        await client.DisposeAsync();

    }
}
