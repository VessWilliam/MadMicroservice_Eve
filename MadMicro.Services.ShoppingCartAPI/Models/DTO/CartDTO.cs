namespace MadMicro.Services.ShoppingCartAPI.Models.DTO;

public class CartDTO
{
    public CartHeadersDTO CartHeaders { get; set; }
    public IEnumerable<CartDetailsDTO>? CartDetails { get; set; }

}
