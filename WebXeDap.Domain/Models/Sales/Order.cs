namespace WebXeDap.Domain.Models;

public class Order : AuditableEntity
{
	public int ID { get; set; }
	public int UserID { get; set; }
	public required User User { get; set; }
	public DateTime OrderDate { get; set; }
	public decimal SubTotal { get; set; }
	public decimal TotalAmount { get; set; }
	public ICollection<OrderItem> OrderItems { get; set; } = [];
}
