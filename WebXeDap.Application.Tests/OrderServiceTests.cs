using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Orders;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Tests;

public sealed class OrderServiceTests
{
	[Fact]
	public async Task CreateOrderAsync_CreatesOrderAndClearsCart()
	{
		using var context = ServiceTestHelpers.CreateContext();
		var user = new User { ID = 1, Email = "buyer@example.com" };
		var product = new Product
		{
			Name = "Mountain Bike",
			Price = 2000m,
			CurrencySymbol = "VNĐ",
			Quantity = 5
		};

		context.Users.Add(user);
		context.Products.Add(product);
		await context.SaveChangesAsync(CancellationToken.None);

		var cartItem = new CartItem
		{
			UserID = user.ID,
			User = user,
			ProductID = product.ID,
			Product = product,
			Quantity = 2
		};

		context.CartItems.Add(cartItem);
		await context.SaveChangesAsync(CancellationToken.None);

		var currentUser = new TestCurrentUserService { UserId = 1 };
		var service = new OrderService(context, currentUser);

		var orderId = await service.CreateOrderAsync(CancellationToken.None);

		var order = await context.Orders
			.Include(o => o.OrderItems)
			.FirstOrDefaultAsync(o => o.ID == orderId);

		var remainingCartItems = await context.CartItems.CountAsync();
		var refreshedProduct = await context.Products.FirstAsync(p => p.ID == product.ID);

		Assert.NotNull(order);
		Assert.Single(order!.OrderItems);
		Assert.Equal(0, remainingCartItems);
		Assert.Equal(3, refreshedProduct.Quantity);
	}
}
