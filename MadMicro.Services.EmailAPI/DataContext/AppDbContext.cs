using MadMicro.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MadMicro.Services.EmailAPI.DataContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<EmailLogger> EmailLoggers { get; set; }

}
