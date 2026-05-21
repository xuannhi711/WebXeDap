using Microsoft.EntityFrameworkCore;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Cart.DTOs;
using WebXeDap.Application.UnitTests.Extensions;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.UnitTests.Cart;

[Trait("Category", "Unit")]
public class CartServiceListTests
{
	private readonly IApplicationDbContext ctx;
	private readonly ICartService service;
	private readonly TestCurrentUserService currentUser;

	public CartServiceListTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<ICartService>();
		currentUser = fixture.GetService<TestCurrentUserService>();
	}

	[Fact]
	public async Task ListAsync_ReturnsNotFound_WhenNoUser()
	{
		var result = await service.ListAsync();

		Assert.True(result.TryPickError(out var error));
		var unauthzErr = Assert.IsType<UnauthorizedError>(error);
		Assert.Equal("User is not authenticated.", unauthzErr.Message);
	}

	[Fact]
	public async Task ListAsync_ReturnsItems_WhenUserHasCart()
	{
		var user = await ctx.AddRandomUserAsync();
		currentUser.UserID = user.Id;

		var product = await ctx.AddRandomProductAsync();
		var cartItem = new CartItem
		{
			UserID = user.Id,
			User = user,
			ProductID = product.ID,
			Product = product,
			Quantity = 2,
		};
		await ctx.AddCartItemAsync(cartItem);

		var result = await service.ListAsync();

		Assert.True(result.TryPickValue(out var items));
		Assert.Single(items);
		var item = items[0];
		Assert.Equal(cartItem.ID, item.ID);
		Assert.Equal(product.ID, item.Product.ID);
		Assert.Equal(product.Name, item.Product.Name);
		Assert.Equal(cartItem.Quantity, item.Quantity);
	}
}

public class CartServiceAddTests
{
	private readonly IApplicationDbContext ctx;
	private readonly ICartService service;
	private readonly TestCurrentUserService currentUser;

	public CartServiceAddTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<ICartService>();
		currentUser = fixture.GetService<TestCurrentUserService>();
	}

	[Fact]
	public async Task AddAsync_AddsItem_WhenProductExists()
	{
		var user = await ctx.AddRandomUserAsync();
		currentUser.UserID = user.Id;
		var product = await ctx.AddRandomProductAsync();

		var cmd = new AddCartItemCommand(product.ID, 2);
		var result = await service.AddAsync(cmd);

		Assert.True(result.TryPickValue(out var item));
		Assert.Equal(product.ID, item.Product.ID);
		Assert.Equal(2, item.Quantity);
	}

	[Fact]
	public async Task AddAsync_IncrementsQuantity_WhenItemExists()
	{
		var user = await ctx.AddRandomUserAsync();
		currentUser.UserID = user.Id;

		var product = await ctx.AddRandomProductAsync();

		var cartItem = new CartItem
		{
			UserID = user.Id,
			User = user,
			ProductID = product.ID,
			Product = product,
			Quantity = 1,
		};
		await ctx.AddCartItemAsync(cartItem);

		var cmd = new AddCartItemCommand(product.ID, 2);
		var result = await service.AddAsync(cmd);

		Assert.True(result.TryPickValue(out var item));
		Assert.Equal(cartItem.ID, item.ID);
		Assert.Equal(3, item.Quantity);

		var stored = await ctx.CartItems.SingleAsync(i =>
			i.UserID == user.Id && i.ProductID == product.ID
		);
		Assert.Equal(3, stored.Quantity);
	}
}

public class CartServiceUpdateTests
{
	private readonly IApplicationDbContext ctx;
	private readonly ICartService service;
	private readonly TestCurrentUserService currentUser;

	public CartServiceUpdateTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<ICartService>();
		currentUser = fixture.GetService<TestCurrentUserService>();
	}

	[Fact]
	public async Task UpdateAsync_UpdatesQuantity_WhenItemExists()
	{
		var user = await ctx.AddRandomUserAsync();
		currentUser.UserID = user.Id;
		var product = await ctx.AddRandomProductAsync();

		var cartItem = new CartItem
		{
			UserID = user.Id,
			User = user,
			ProductID = product.ID,
			Product = product,
			Quantity = 1,
		};
		await ctx.AddCartItemAsync(cartItem);

		var cmd = new UpdateCartItemCommand(cartItem.ID, 4);
		var result = await service.UpdateAsync(cmd);

		Assert.True(result.TryPickValue(out var item));
		Assert.Equal(cartItem.ID, item.ID);
		Assert.Equal(4, item.Quantity);

		var stored = await ctx.CartItems.SingleAsync(i =>
			i.UserID == user.Id && i.ProductID == product.ID
		);
		Assert.Equal(4, stored.Quantity);
	}
}

public class CartServiceDeleteTests
{
	private readonly IApplicationDbContext ctx;
	private readonly ICartService service;
	private readonly TestCurrentUserService currentUser;

	public CartServiceDeleteTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<ICartService>();
		currentUser = fixture.GetService<TestCurrentUserService>();
	}

	[Fact]
	public async Task DeleteAsync_RemovesItem()
	{
		var user = await ctx.AddRandomUserAsync();
		currentUser.UserID = user.Id;
		var product = await ctx.AddRandomProductAsync();
		var cartItem = new CartItem
		{
			UserID = user.Id,
			User = user,
			ProductID = product.ID,
			Product = product,
			Quantity = 1,
		};
		await ctx.AddCartItemAsync(cartItem);

		var result = await service.DeleteAsync(cartItem.ID);

		Assert.True(result.IsOk);
		var deleted = await ctx.CartItems.FirstOrDefaultAsync(i =>
			i.UserID == user.Id && i.ProductID == product.ID
		);
		Assert.Null(deleted);
	}
}
