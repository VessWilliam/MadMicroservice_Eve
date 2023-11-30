namespace MadMicro.Services.ProductAPI.Models.DTO;

public class ProductDTO
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string? ImageURL { get; set; }
    public string? ImageLocalPath { get; set; }
    public IFormFile? Image {  get; set; }

}
