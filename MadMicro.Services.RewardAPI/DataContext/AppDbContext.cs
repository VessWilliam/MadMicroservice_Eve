using MadMicro.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MadMicro.Services.RewardAPI.DataContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Rewards> Rewards { get; set; }

}
