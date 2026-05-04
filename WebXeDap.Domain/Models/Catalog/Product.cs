namespace WebXeDap.Domain.Models;

public class Product : BaseAuditableEntity
{
	public int ID { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }
	public decimal Price { get; set; }
	public string CurrencySymbol { get; set; } = "VNĐ";
	public int Quantity { get; set; }
	public ICollection<Category> Categories { get; set; } = [];
	public ICollection<ProductImage> Images { get; set; } = [];
}
