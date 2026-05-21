using WebXeDap.Domain.Enums;
using WebXeDap.Domain.Models;

namespace WebXeDap.Domain.UnitTests;

[Trait("Category", "Unit")]
public sealed class ProductPricingTests
{
	[Fact]
	public void GetEffectivePrice_ReturnsBasePrice_WhenNoCampaigns()
	{
		var product = new Product { Name = "Bike", Price = 100m };

		var price = product.GetEffectivePrice(DateTime.UtcNow);

		Assert.Equal(100m, price);
	}

	[Fact]
	public void GetEffectivePrice_UsesBestActiveDiscount()
	{
		var now = DateTime.UtcNow;
		var product = new Product { Name = "Bike", Price = 100m };
		product.SaleCampaigns.Add(
			new SaleCampaign
			{
				Name = "Ten Percent",
				DiscountType = DiscountType.Percentage,
				DiscountValue = 10,
				StartsAt = now.AddDays(-1),
				EndsAt = now.AddDays(1),
			}
		);
		product.SaleCampaigns.Add(
			new SaleCampaign
			{
				Name = "Fixed",
				DiscountType = DiscountType.FixedAmount,
				DiscountValue = 15,
				StartsAt = now.AddDays(-1),
				EndsAt = now.AddDays(1),
			}
		);

		var price = product.GetEffectivePrice(now);

		Assert.Equal(85m, price);
	}

	[Fact]
	public void GetEffectivePrice_IgnoresInactiveCampaigns()
	{
		var now = DateTime.UtcNow;
		var product = new Product { Name = "Bike", Price = 100m };
		product.SaleCampaigns.Add(
			new SaleCampaign
			{
				Name = "Inactive",
				DiscountType = DiscountType.FixedAmount,
				DiscountValue = 25,
				StartsAt = now.AddDays(-10),
				EndsAt = now.AddDays(-5),
			}
		);

		var price = product.GetEffectivePrice(now);

		Assert.Equal(100m, price);
	}

	[Fact]
	public void GetEffectivePrice_NeverBelowZero()
	{
		var now = DateTime.UtcNow;
		var product = new Product { Name = "Bike", Price = 20m };
		product.SaleCampaigns.Add(
			new SaleCampaign
			{
				Name = "Huge Discount",
				DiscountType = DiscountType.FixedAmount,
				DiscountValue = 50,
				StartsAt = now.AddDays(-1),
				EndsAt = now.AddDays(1),
			}
		);

		var price = product.GetEffectivePrice(now);

		Assert.Equal(0m, price);
	}
}
