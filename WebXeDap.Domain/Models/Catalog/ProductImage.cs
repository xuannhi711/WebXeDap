namespace WebXeDap.Domain.Models;

public class ProductImage
{
	public int ID { get; set; }
	public required string key { get; set; }
	public int Order { get; set; }
	public int ProductID { get; set; }
	public required Product Product { get; set; }
}
