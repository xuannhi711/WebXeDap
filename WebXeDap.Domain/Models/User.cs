namespace WebXeDap.Domain.Models;

public class User
{
	public int ID { get; set; }
	public required string Email { get; set; }
	public string? FullName { get; set; }
	public string? Address { get; set; }
	public string? Avatar { get; set; }
	public ICollection<Order> Orders { get; set; } = [];
	public ICollection<Notification> Notifications { get; set; } = [];
	public ICollection<CartItem> CartItems { get; set; } = [];
}
