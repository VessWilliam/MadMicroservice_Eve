using MadMicro.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MadMicro.Services.OrderAPI.DataContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<OrderHeader> OrderHeaders { get; set; }
    public DbSet<OrderDetails> OrderDetails { get; set; }
}
