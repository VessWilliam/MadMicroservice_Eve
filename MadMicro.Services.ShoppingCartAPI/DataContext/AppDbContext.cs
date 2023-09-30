using MadMicro.Services.ShoppingCartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MadMicro.Services.ShoppingCartAPI.DataContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<CartHeaders> CartHeaders { get; set; }
    public DbSet<CartDetails> CartDetails { get; set; }
}
