using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.UnitTests.Extensions;

public static class CartItemDbCtxExtensions
{
	public static async Task AddCartItemAsync(this IApplicationDbContext ctx, CartItem cartItem)
	{
		await ctx.CartItems.AddAsync(cartItem);
		await ctx.SaveChangesAsync(default);
	}
}
