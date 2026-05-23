namespace WebXeDap.Domain.Models.Maintenance;

public class MaintenancePlan : BaseAuditableEntity
{
    public int ID { get; set; }
    public int OrderItemID { get; set; }
    public OrderItem OrderItem { get; set; } = null!;
    public string Note { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public int IntervalInDays { get; set; }
}