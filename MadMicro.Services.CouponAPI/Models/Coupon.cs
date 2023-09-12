using System.ComponentModel.DataAnnotations;

namespace MadMicro.Services.CouponAPI.Models;

public class Coupon
{
    [Key]
    public int CouponId { get; set; }
    [Required]
    public string CouponCode { get; set; }
    [Required]
    public double DiscountAmount { get; set; }
    [Required]
    public int MinAmount { get; set; }

}
