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
}
