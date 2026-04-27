using WebXeDap.Domain.Common;

namespace WebXeDap.Domain.Models.Catalog;

public class ProductImage
{
	public int ID { get; set; }
	public required string key { get; set; }
	public int Order { get; set; }
	public int ProductID { get; set; }
	public Product? Product { get; set; }
}
