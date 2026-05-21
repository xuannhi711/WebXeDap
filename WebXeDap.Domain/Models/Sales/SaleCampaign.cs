using WebXeDap.Domain.Enums;

namespace WebXeDap.Domain.Models;

public class SaleCampaign : BaseAuditableEntity
{
	public int ID { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }
	public DiscountType DiscountType { get; set; }
	public decimal DiscountValue { get; set; }
	public DateTime StartsAt { get; set; }
	public DateTime EndsAt { get; set; }
	public ICollection<Product> Products { get; set; } = [];

	public bool IsActive(DateTime now)
	{
		return !IsDeleted && StartsAt <= now && EndsAt >= now;
	}

	public decimal GetDiscountAmount(decimal basePrice)
	{
		if (basePrice <= 0)
		{
			return 0;
		}

		return DiscountType switch
		{
			DiscountType.Percentage => basePrice * DiscountValue / 100m,
			DiscountType.FixedAmount => DiscountValue,
			_ => 0,
		};
	}
}
