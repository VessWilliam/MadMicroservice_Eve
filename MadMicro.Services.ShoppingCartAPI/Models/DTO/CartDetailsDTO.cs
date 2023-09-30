namespace MadMicro.Services.ShoppingCartAPI.Models.DTO;

public class CartDetailsDTO
{
    public int CartDetailsId { get; set; }
    public int CartHeaderId { get; set; }
    public int ProductId { get; set; }
    public int Count { get; set; }
    public ProductDTO? Product { get; set; }
    public CartHeadersDTO? CartHeader { get; set; }
}
