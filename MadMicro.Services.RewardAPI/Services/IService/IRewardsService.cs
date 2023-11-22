using MadMicro.Services.RewardAPI.Message;

namespace MadMicro.Services.RewardAPI.Services.IService;

public interface IRewardsService
{
    Task UpdateRewards(RewardMessage rewardMessage);

}
