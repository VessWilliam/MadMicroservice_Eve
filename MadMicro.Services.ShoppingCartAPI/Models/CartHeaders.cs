using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MadMicro.Services.ShoppingCartAPI.Models;

public class CartHeaders
{
    [Key]
    public int CartHeaderId { get; set; }
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }

    [NotMapped]
    public double Discount { get; set; }
    [NotMapped]
    public double CartTotal { get; set; }
}
