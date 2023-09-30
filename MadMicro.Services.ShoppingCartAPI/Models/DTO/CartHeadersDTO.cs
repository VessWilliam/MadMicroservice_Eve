namespace MadMicro.Services.ShoppingCartAPI.Models.DTO;

public class CartHeadersDTO
{
    public int CartHeaderId { get; set; }
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }
    public double Discount { get; set; }
    public double CartTotal { get; set; }
}
