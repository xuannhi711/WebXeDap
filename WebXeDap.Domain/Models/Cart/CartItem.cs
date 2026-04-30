namespace WebXeDap.Domain.Models;

public class CartItem
{
	public int ID { get; set; }
	public int UserID { get; set; }
	public required User User { get; set; }
	public int ProductID { get; set; }
	public required Product Product { get; set; }
	public int Quantity { get; set; }
}
