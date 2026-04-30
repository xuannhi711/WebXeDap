namespace WebXeDap.Domain.Models;

public class OrderItem
{
	public int ID { get; set; }
	public int ProductID { get; set; }
	public required Product Product { get; set; }
	public int OrderID { get; set; }
	public required Order Order { get; set; }
	public int Quantity { get; set; }
	public decimal UnitPrice { get; set; }
}
