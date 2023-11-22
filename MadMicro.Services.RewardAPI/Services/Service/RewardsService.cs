using MadMicro.Services.RewardAPI.DataContext;
using MadMicro.Services.RewardAPI.Message;
using MadMicro.Services.RewardAPI.Models;
using MadMicro.Services.RewardAPI.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace MadMicro.Services.RewardAPI.Services.Service;

public class RewardsService : IRewardsService
{
    private readonly DbContextOptions<AppDbContext> _dbContext;

    public RewardsService(DbContextOptions<AppDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpdateRewards(RewardMessage rewardMessage)
    {
        try
        {
            Rewards rewards = new()
            {
                OrderId = rewardMessage.OrderId,
                RewardsActivity = rewardMessage.RewardsActivity,
                UserId = rewardMessage.UserId,  
                RewardsDate = DateTime.Now,
            };
            await using var _context = new AppDbContext(_dbContext);
            await _context.Rewards.AddAsync(rewards);
            await _context.SaveChangesAsync();  
        }
        catch (Exception)
        {

            throw;
        }
    }
}

