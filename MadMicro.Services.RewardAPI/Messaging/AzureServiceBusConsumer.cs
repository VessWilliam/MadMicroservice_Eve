using Azure.Messaging.ServiceBus;
using MadMicro.Services.RewardAPI.Message;
using MadMicro.Services.RewardAPI.Services.IService;
using MadMicro.Services.RewardAPI.Services.Service;
using Newtonsoft.Json;
using System.Text;

namespace MadMicro.Services.RewardAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string orderCreatedTopic;
    private readonly string orderCreatedRewardSubscription;
    private readonly IConfiguration _config;
    private readonly RewardsService _rewardService;   

    private ServiceBusProcessor _rewardProcessor;


    public AzureServiceBusConsumer(IConfiguration config, RewardsService rewardService)
    {
        _config = config;
        serviceBusConnectionString =
            _config.GetValue<string>("ServiceBusConnectionString") ?? string.Empty;

        orderCreatedTopic = _config.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic") ?? string.Empty;
        orderCreatedRewardSubscription = _config.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription") ?? string.Empty;

        var clientBus = new ServiceBusClient(serviceBusConnectionString);
        _rewardProcessor = clientBus.CreateProcessor(orderCreatedTopic, orderCreatedRewardSubscription);
        _rewardService = rewardService;   
    }
    public async Task Start()
    {
        _rewardProcessor.ProcessMessageAsync += OnOrderRewardsRequestReceived;
        _rewardProcessor.ProcessErrorAsync += ErrorHandler;
        await _rewardProcessor.StartProcessingAsync();
      
    }

    public async Task Stop()
    {
       await _rewardProcessor.StopProcessingAsync();
       await _rewardProcessor.DisposeAsync();
    }


    private async Task OnOrderRewardsRequestReceived(ProcessMessageEventArgs msg)
    {
        var message = msg.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        var objMesage = JsonConvert.DeserializeObject<RewardMessage>(body);

        try
        {
            await _rewardService.UpdateRewards(objMesage);
            await msg.CompleteMessageAsync(msg.Message);
        }
        catch (Exception)
        {

            throw;
        }

    }

    private Task ErrorHandler(ProcessErrorEventArgs error)
    {
        Console.WriteLine(error.Exception.ToString());
        return Task.CompletedTask;
    }

   
}
