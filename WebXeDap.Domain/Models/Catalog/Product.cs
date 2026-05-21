namespace WebXeDap.Domain.Models;

public class Product : BaseAuditableEntity
{
	public int ID { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }
	public decimal Price { get; set; }
	public string CurrencySymbol { get; set; } = "VNĐ";
	public int Quantity { get; set; }
	public ICollection<Category> Categories { get; set; } = [];
	public ICollection<ProductImage> Images { get; set; } = [];
	public ICollection<SaleCampaign> SaleCampaigns { get; set; } = [];

	public decimal GetEffectivePrice(DateTime? now = null)
	{
		var effectiveNow = now ?? DateTime.UtcNow;
		if (SaleCampaigns.Count == 0)
		{
			return Price;
		}

		decimal bestDiscount = 0;
		foreach (var campaign in SaleCampaigns)
		{
			if (!campaign.IsActive(effectiveNow))
			{
				continue;
			}

			var discount = campaign.GetDiscountAmount(Price);
			if (discount > bestDiscount)
			{
				bestDiscount = discount;
			}
		}

		var discountedPrice = Price - bestDiscount;
		return discountedPrice < 0 ? 0 : discountedPrice;
	}
}
