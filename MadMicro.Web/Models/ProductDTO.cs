using System.ComponentModel.DataAnnotations;

namespace MadMicro.Web.Models;

public class ProductDTO
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string ImageURL { get; set; }
    [Range(1,100)]
    public int Count { get; set; } = 1;

}
