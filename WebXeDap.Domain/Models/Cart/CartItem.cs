namespace WebXeDap.Domain.Models;

public class CartItem
{
	public int ID { get; set; }
	public int UserID { get; set; }
	public User User { get; set; } = null!;
	public int ProductID { get; set; }
	public Product Product { get; set; } = null!;
	public int Quantity { get; set; }
}
