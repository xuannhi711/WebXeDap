using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Cart;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Tests;

public sealed class CartServiceTests
{
	[Fact]
	public async Task AddToCartAsync_AddsNewItem()
	{
		using var context = ServiceTestHelpers.CreateContext();
		var user = new User { ID = 1, Email = "test@example.com" };
		var product = new Product
		{
			Name = "Road Bike",
			Price = 1500m,
			CurrencySymbol = "VNĐ",
			Quantity = 10
		};

		context.Users.Add(user);
		context.Products.Add(product);
		await context.SaveChangesAsync(CancellationToken.None);

		var currentUser = new TestCurrentUserService { UserId = 1 };
		var service = new CartService(context, currentUser);

		var cartItemId = await service.AddToCartAsync(product.ID, 2, CancellationToken.None);

		var cartItem = await context.CartItems
			.AsNoTracking()
			.FirstOrDefaultAsync(ci => ci.ID == cartItemId);

		Assert.NotNull(cartItem);
		Assert.Equal(product.ID, cartItem!.ProductID);
		Assert.Equal(2, cartItem.Quantity);
	}
}
