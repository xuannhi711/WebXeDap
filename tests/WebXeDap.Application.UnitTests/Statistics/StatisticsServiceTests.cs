using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Statistics.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.UnitTests.Statistics;

[Trait("Category", "Unit")]
public class StatisticsServiceTests
{
	private readonly IApplicationDbContext ctx;
	private readonly IStatisticsService service;

	public StatisticsServiceTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<IStatisticsService>();
	}

	[Fact]
	public async Task GetOverviewAsync_ReturnsCorrectCountsAndRevenue()
	{
		// arrange
		var user1 = new User { Email = "a@example.com", FullName = "A" };
		var user2 = new User { Email = "b@example.com", FullName = "B" };
		await ctx.Users.AddRangeAsync(user1, user2);

		var p1 = new Product
		{
			Name = "P1",
			Price = 10m,
			Quantity = 5,
		};
		var p2 = new Product
		{
			Name = "P2",
			Price = 20m,
			Quantity = 3,
		};
		var p3 = new Product
		{
			Name = "P3",
			Price = 5m,
			Quantity = 10,
		};
		await ctx.Products.AddRangeAsync(p1, p2, p3);

		var order1 = new Order
		{
			User = user1,
			OrderDate = DateTime.UtcNow.AddDays(-2),
			TotalAmount = 100m,
		};
		var order2 = new Order
		{
			User = user2,
			OrderDate = DateTime.UtcNow.AddDays(-1),
			TotalAmount = 200m,
		};
		await ctx.Orders.AddRangeAsync(order1, order2);

		var now = DateTime.UtcNow;
		var campaign = new SaleCampaign
		{
			Name = "S",
			StartsAt = now.AddDays(-1),
			EndsAt = now.AddDays(1),
			DiscountType = WebXeDap.Domain.Enums.DiscountType.Percentage,
			DiscountValue = 10,
		};
		await ctx.SaleCampaigns.AddAsync(campaign);

		await ctx.SaveChangesAsync(default);

		// act
		var overview = await service.GetOverviewAsync();

		// assert
		Assert.Equal(2, overview.TotalUsers);
		Assert.Equal(3, overview.TotalProducts);
		Assert.Equal(2, overview.TotalOrders);
		Assert.Equal(300m, overview.TotalRevenue);
		Assert.Equal(1, overview.ActiveSaleCampaigns);
	}

	[Fact]
	public async Task GetTopProductsAsync_ReturnsProductsOrderedByQuantity()
	{
		// arrange
		var p1 = new Product
		{
			Name = "Product A",
			Price = 10m,
			Quantity = 10,
		};
		var p2 = new Product
		{
			Name = "Product B",
			Price = 20m,
			Quantity = 10,
		};
		await ctx.Products.AddRangeAsync(p1, p2);
		await ctx.SaveChangesAsync(default);

		var order = new Order
		{
			User = new User { Email = "x@x.com", FullName = "X" },
			OrderDate = DateTime.UtcNow,
			TotalAmount = 0m,
		};
		await ctx.Orders.AddAsync(order);
		await ctx.SaveChangesAsync(default);

		var oi1 = new OrderItem
		{
			OrderID = order.ID,
			ProductID = p1.ID,
			Quantity = 5,
			UnitPrice = 10m,
		};
		var oi2 = new OrderItem
		{
			OrderID = order.ID,
			ProductID = p2.ID,
			Quantity = 3,
			UnitPrice = 20m,
		};
		await ctx.OrderItems.AddRangeAsync(oi1, oi2);
		await ctx.SaveChangesAsync(default);

		// act
		var top = await service.GetTopProductsAsync(2);

		// assert
		Assert.Equal(2, top.Count);
		Assert.Equal(p1.ID, top[0].ProductID);
		Assert.Equal(5, top[0].QuantitySold);
		Assert.Equal(50m, top[0].Revenue);

		Assert.Equal(p2.ID, top[1].ProductID);
		Assert.Equal(3, top[1].QuantitySold);
		Assert.Equal(60m, top[1].Revenue);
	}
}
