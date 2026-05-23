namespace WebXeDap.Domain.Models;

public class Order : BaseAuditableEntity
{
	public int ID { get; set; }
	public int UserID { get; set; }
	public User User { get; set; } = null!;
	public DateTime OrderDate { get; set; }
	public decimal SubTotal { get; set; }
	public decimal TotalAmount { get; set; }
	public ICollection<OrderItem> OrderItems { get; set; } = [];
	public ICollection<Payment> Payments { get; set; } = [];
}
