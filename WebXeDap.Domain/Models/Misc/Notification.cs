using WebXeDap.Domain.Enums;

namespace WebXeDap.Domain.Models;

public class Notification
{
	public int ID { get; set; }
	public int UserID { get; set; }
	public required User User { get; set; }
	public NotificationType Type { get; set; }
	public required string Message { get; set; }
	public bool IsSent { get; set; }
	public DateTime SentAt { get; set; }
}
