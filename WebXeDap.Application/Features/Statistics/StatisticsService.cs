using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Statistics.DTOs;

namespace WebXeDap.Application.Features.Statistics;

public class StatisticsService : IStatisticsService
{
	private readonly IApplicationDbContext ctx;

	public StatisticsService(IApplicationDbContext ctx)
	{
		this.ctx = ctx;
	}

	public async Task<StatisticsOverviewResponse> GetOverviewAsync(DateTime? since = null)
	{
		var now = DateTime.UtcNow;
		var totalUsers = await ctx.Users.CountAsync();
		var totalProducts = await ctx.Products.CountAsync();
		var totalOrders = await ctx.Orders.CountAsync();

		decimal totalRevenue = 0m;
		if (since.HasValue)
		{
			totalRevenue =
				await ctx
					.Orders.Where(o => o.OrderDate >= since.Value)
					.SumAsync(o => (decimal?)o.TotalAmount)
				?? 0m;
		}
		else
		{
			totalRevenue = await ctx.Orders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
		}

		var activeCampaigns = await ctx.SaleCampaigns.CountAsync(s =>
			!s.IsDeleted && s.StartsAt <= now && s.EndsAt >= now
		);

		return new StatisticsOverviewResponse(
			totalUsers,
			totalProducts,
			totalOrders,
			totalRevenue,
			activeCampaigns
		);
	}

	public async Task<List<TopProductResponse>> GetTopProductsAsync(int limit = 10)
	{
		var q = ctx
			.OrderItems.AsNoTracking()
			.GroupBy(oi => oi.ProductID)
			.Select(g => new
			{
				ProductID = g.Key,
				Quantity = g.Sum(x => x.Quantity),
				Revenue = g.Sum(x => x.Quantity * x.UnitPrice),
			})
			.OrderByDescending(x => x.Quantity)
			.Take(limit);

		var list = await q.ToListAsync();

		// load product names
		var productIds = list.Select(x => x.ProductID).ToList();
		var products = await ctx
			.Products.Where(p => productIds.Contains(p.ID))
			.ToDictionaryAsync(p => p.ID, p => p.Name);

		return list.Select(x => new TopProductResponse(
				x.ProductID,
				products.TryGetValue(x.ProductID, out var n) ? n : string.Empty,
				x.Quantity,
				x.Revenue
			))
			.ToList();
	}
}
