using MadMicro.Services.ShoppingCartAPI.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadMicro.Services.ShoppingCartAPI.Models;
public class CartDetails
{
    [Key]
    public int CartDetailsId { get; set; }
    public int CartHeaderId { get; set; }
    [ForeignKey("CartHeaderId")]
    public CartHeaders CartHeader { get; set; }
    public int ProductId { get; set; }
    [NotMapped]
    public ProductDTO productDTO { get; set; }
    public int Count { get; set; }
}

