using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.UnitTests.Extensions;

public static class CategoryCtxExtensions
{
	public static async Task AddCategoryAsync(this IApplicationDbContext ctx, Category category)
	{
		await ctx.Categories.AddAsync(category);
		await ctx.SaveChangesAsync(default);
	}

	public static async Task AddCategoriesAsync(
		this IApplicationDbContext ctx,
		IEnumerable<Category> categories
	)
	{
		await ctx.Categories.AddRangeAsync(categories);
		await ctx.SaveChangesAsync(default);
	}

	public static async Task<Category> AddRandomCategoryAsync(this IApplicationDbContext ctx)
	{
		var category = new Category { Name = Guid.NewGuid().ToString() };
		await ctx.AddCategoryAsync(category);
		return category;
	}
}
