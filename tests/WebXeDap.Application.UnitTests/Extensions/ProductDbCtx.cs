using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.UnitTests.Extensions;

public static class ProductCtxExtensions
{
	public static async Task AddProductAsync(this IApplicationDbContext ctx, Product product)
	{
		await ctx.Products.AddAsync(product);
		await ctx.SaveChangesAsync(default);
	}

	public static async Task<Product> AddRandomProductAsync(this IApplicationDbContext ctx)
	{
		const decimal MIN_PRICE = 1m;
		const decimal MAX_PRICE = 1000m;

		var product = new Product
		{
			Name = Guid.NewGuid().ToString(),
			Price = (decimal)Random.Shared.NextDouble() * (MAX_PRICE - MIN_PRICE) + MIN_PRICE,
			Quantity = Random.Shared.Next(1, 100),
		};
		await ctx.AddProductAsync(product);
		return product;
	}
}
