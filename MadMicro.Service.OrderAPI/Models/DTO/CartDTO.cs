namespace MadMicro.Service.OrderAPI;

public class CartDTO
{
    public CartHeadersDTO CartHeaders { get; set; }
    public IEnumerable<CartDetailsDTO>? CartDetails { get; set; }

}
