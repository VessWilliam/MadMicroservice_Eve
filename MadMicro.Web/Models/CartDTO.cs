namespace MadMicro.Web.Models;

public class CartDTO
{
    public CartHeadersDTO CartHeaders { get; set; }
    public IEnumerable<CartDetailsDTO>? CartDetails { get; set; }

}
