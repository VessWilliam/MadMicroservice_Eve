using MadMicro.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MadMicro.Services.CouponAPI.DataContext;

public class AppDbContext :DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Coupon> Coupons { get; set; }
}
