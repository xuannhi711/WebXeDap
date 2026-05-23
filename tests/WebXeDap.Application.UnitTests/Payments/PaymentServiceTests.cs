using Microsoft.EntityFrameworkCore;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Payments.DTOs;
using WebXeDap.Application.UnitTests.Extensions;
using WebXeDap.Domain.Enums;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.UnitTests.Payments;

[Trait("Category", "Unit")]
public sealed class PaymentServiceCheckoutTests
{
	private readonly IApplicationDbContext ctx;
	private readonly IPaymentService service;
	private readonly TestCurrentUserService currentUser;

	public PaymentServiceCheckoutTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<IPaymentService>();
		currentUser = fixture.GetService<TestCurrentUserService>();
	}

	[Fact]
	public async Task CheckoutAsync_Pass_WhenSelectionIsValid()
	{
		var user = await ctx.AddRandomUserAsync();
		currentUser.UserID = user.Id;

		var product1 = new Product
		{
			Name = "Road Bike",
			Price = 100m,
			Quantity = 10,
		};
		var product2 = new Product
		{
			Name = "Helmet",
			Price = 25m,
			Quantity = 15,
		};
		await ctx.AddProductAsync(product1);
		await ctx.AddProductAsync(product2);

		var cartItem1 = new CartItem
		{
			UserID = user.Id,
			User = user,
			ProductID = product1.ID,
			Product = product1,
			Quantity = 2,
		};
		var cartItem2 = new CartItem
		{
			UserID = user.Id,
			User = user,
			ProductID = product2.ID,
			Product = product2,
			Quantity = 1,
		};
		await ctx.AddCartItemAsync(cartItem1);
		await ctx.AddCartItemAsync(cartItem2);

		var result = await service.CheckoutAsync(
			new CheckoutPaymentCommand([cartItem1.ID, cartItem2.ID], PaymentProvider.Manual)
		);

		Assert.True(result.TryPickValue(out var payment));
		Assert.Equal(PaymentStatus.Pending, payment.Status);
		Assert.Equal(PaymentProvider.Manual, payment.Provider);
		Assert.Equal(225m, payment.Amount);
		Assert.Equal("VND", payment.CurrencyCode);

		var order = await ctx
			.Orders.Include(o => o.OrderItems)
			.FirstOrDefaultAsync(o => o.ID == payment.OrderID);
		Assert.NotNull(order);
		Assert.Equal(user.Id, order!.UserID);
		Assert.Equal(225m, order.TotalAmount);
		Assert.Equal(2, order.OrderItems.Count);
		Assert.Collection(
			order.OrderItems.OrderBy(i => i.ProductID),
			item =>
			{
				Assert.Equal(product1.ID, item.ProductID);
				Assert.Equal(2, item.Quantity);
				Assert.Equal(100m, item.UnitPrice);
			},
			item =>
			{
				Assert.Equal(product2.ID, item.ProductID);
				Assert.Equal(1, item.Quantity);
				Assert.Equal(25m, item.UnitPrice);
			}
		);

		var storedPayment = await ctx.Payments.FirstOrDefaultAsync(p => p.ID == payment.ID);
		Assert.NotNull(storedPayment);
		Assert.StartsWith($"PAY-{order.ID}-", storedPayment!.ReferenceCode);
		Assert.Equal(order.ID, storedPayment.OrderID);
	}

	[Fact]
	public async Task CreateAsync_UsesOrderTotalAmount_WhenAmountIsNull()
	{
		var order = new Order
		{
			UserID = 1,
			OrderDate = DateTime.UtcNow,
			SubTotal = 455m,
			TotalAmount = 455m,
		};
		await ctx.Orders.AddAsync(order);
		await ctx.SaveChangesAsync(default);

		var result = await service.CreateAsync(
			new CreatePaymentCommand(order.ID, PaymentProvider.Manual, null, "vnd")
		);

		Assert.True(result.TryPickValue(out var payment));
		Assert.Equal(455m, payment.Amount);
		Assert.Equal("VND", payment.CurrencyCode);
	}

	[Fact]
	public async Task CreateAsync_Fails_WhenOrderDoesNotExist()
	{
		var result = await service.CreateAsync(
			new CreatePaymentCommand(999, PaymentProvider.Manual, 100m, "vnd")
		);

		Assert.True(result.TryPickError(out var error));
		Assert.IsType<NotFoundError>(error);
	}

	[Fact]
	public async Task CheckoutAsync_Fail_WhenSelectedCartItemBelongsToAnotherUser()
	{
		var owner = await ctx.AddRandomUserAsync();
		var stranger = await ctx.AddRandomUserAsync();
		currentUser.UserID = owner.Id;

		var product = new Product
		{
			Name = "Frame",
			Price = 500m,
			Quantity = 5,
		};
		await ctx.AddProductAsync(product);

		var cartItem = new CartItem
		{
			UserID = stranger.Id,
			User = stranger,
			ProductID = product.ID,
			Product = product,
			Quantity = 1,
		};
		await ctx.AddCartItemAsync(cartItem);

		var result = await service.CheckoutAsync(
			new CheckoutPaymentCommand([cartItem.ID], PaymentProvider.Manual)
		);

		Assert.True(result.TryPickError(out var error));
		Assert.IsType<NotFoundError>(error);
	}

	[Fact]
	public async Task CheckoutAsync_Fail_WhenNoUserIsAuthenticated()
	{
		var result = await service.CheckoutAsync(
			new CheckoutPaymentCommand([1], PaymentProvider.Manual)
		);

		Assert.True(result.TryPickError(out var error));
		Assert.IsType<UnauthorizedError>(error);
	}

	[Fact]
	public async Task CheckoutAsync_Fail_WhenNoCartItemsAreSelected()
	{
		var user = await ctx.AddRandomUserAsync();
		currentUser.UserID = user.Id;

		var result = await service.CheckoutAsync(
			new CheckoutPaymentCommand([], PaymentProvider.Manual)
		);

		Assert.True(result.TryPickError(out var error));
		var validationError = Assert.IsType<ValidationError>(error);
		Assert.True(validationError.Errors.ContainsKey(nameof(CheckoutPaymentCommand.CartItemIDs)));
	}

	[Fact]
	public async Task CreateAsync_Pass_WhenOrderExists()
	{
		var order = new Order
		{
			UserID = 1,
			OrderDate = DateTime.UtcNow,
			SubTotal = 300m,
			TotalAmount = 300m,
		};
		await ctx.Orders.AddAsync(order);
		await ctx.SaveChangesAsync(default);

		var result = await service.CreateAsync(
			new CreatePaymentCommand(order.ID, PaymentProvider.VNPay, null, "vnd")
		);

		Assert.True(result.TryPickValue(out var payment));
		Assert.Equal(order.ID, payment.OrderID);
		Assert.Equal(300m, payment.Amount);
		Assert.Equal(PaymentStatus.Pending, payment.Status);
		Assert.Equal("VND", payment.CurrencyCode);
	}

	[Fact]
	public async Task SetProcessingAsync_UpdatesStatusAndUrl()
	{
		var payment = await AddPendingPaymentAsync();

		var result = await service.SetProcessingAsync(
			payment.ID,
			new SetPaymentProcessingCommand("https://pay.example.com/checkout")
		);

		Assert.True(result.TryPickValue(out var updated));
		Assert.Equal(PaymentStatus.Processing, updated.Status);
		Assert.Equal("https://pay.example.com/checkout", updated.ProviderPaymentUrl);
	}

	[Fact]
	public async Task MarkFailedAsync_UpdatesStatusAndFailureReason()
	{
		var payment = await AddPendingPaymentAsync();

		var result = await service.MarkFailedAsync(
			payment.ID,
			new MarkPaymentFailedCommand("Declined", "{\"status\":\"failed\"}")
		);

		Assert.True(result.TryPickValue(out var updated));
		Assert.Equal(PaymentStatus.Failed, updated.Status);
		Assert.Equal("Declined", updated.FailureReason);
		Assert.NotNull(updated.CompletedAt);
	}

	[Fact]
	public async Task CancelAsync_UpdatesStatusAndStoresResponse()
	{
		var payment = await AddPendingPaymentAsync();

		var result = await service.CancelAsync(
			payment.ID,
			new CancelPaymentCommand("{\"cancelled\":true}")
		);

		Assert.True(result.TryPickValue(out var updated));
		Assert.Equal(PaymentStatus.Cancelled, updated.Status);
		Assert.NotNull(updated.CompletedAt);

		var stored = await ctx.Payments.FirstAsync(p => p.ID == payment.ID);
		Assert.Equal(PaymentStatus.Cancelled, stored.Status);
		Assert.Equal("{\"cancelled\":true}", stored.RawResponse);
	}

	private async Task<Payment> AddPendingPaymentAsync()
	{
		var order = new Order
		{
			UserID = 1,
			OrderDate = DateTime.UtcNow,
			SubTotal = 300m,
			TotalAmount = 300m,
		};
		await ctx.Orders.AddAsync(order);
		await ctx.SaveChangesAsync(default);

		var payment = new Payment
		{
			OrderID = order.ID,
			Provider = PaymentProvider.Manual,
			Status = PaymentStatus.Pending,
			Amount = 300m,
			CurrencyCode = "VND",
			ReferenceCode = $"PAY-{order.ID}-test",
		};
		await ctx.Payments.AddAsync(payment);
		await ctx.SaveChangesAsync(default);
		return payment;
	}
}

[Trait("Category", "Unit")]
public sealed class PaymentServiceStatusTests
{
	private readonly IApplicationDbContext ctx;
	private readonly IPaymentService service;

	public PaymentServiceStatusTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<IPaymentService>();
	}

	[Fact]
	public async Task MarkSucceededAsync_UpdatesStatusAndStoresProviderResponse()
	{
		var order = new Order
		{
			UserID = 1,
			OrderDate = DateTime.UtcNow,
			SubTotal = 100m,
			TotalAmount = 100m,
		};
		await ctx.Orders.AddAsync(order);
		await ctx.SaveChangesAsync(default);

		var payment = new Payment
		{
			OrderID = order.ID,
			Provider = PaymentProvider.VNPay,
			Status = PaymentStatus.Pending,
			Amount = 100m,
			CurrencyCode = "VND",
			ReferenceCode = $"PAY-{order.ID}-test",
		};
		await ctx.Payments.AddAsync(payment);
		await ctx.SaveChangesAsync(default);

		var result = await service.MarkSucceededAsync(
			payment.ID,
			new MarkPaymentSucceededCommand("TXN-001", "{\"status\":\"ok\"}")
		);

		Assert.True(result.TryPickValue(out var updated));
		Assert.Equal(PaymentStatus.Succeeded, updated.Status);
		Assert.Equal("TXN-001", updated.ProviderTransactionID);
		Assert.NotNull(updated.CompletedAt);

		var stored = await ctx.Payments.FirstAsync(p => p.ID == payment.ID);
		Assert.Equal(PaymentStatus.Succeeded, stored.Status);
		Assert.Equal("TXN-001", stored.ProviderTransactionID);
	}

	[Fact]
	public async Task MarkSucceededAsync_Fails_WhenTransactionIdIsMissing()
	{
		var order = new Order
		{
			UserID = 1,
			OrderDate = DateTime.UtcNow,
			SubTotal = 300m,
			TotalAmount = 300m,
		};
		await ctx.Orders.AddAsync(order);
		await ctx.SaveChangesAsync(default);

		var payment = new Payment
		{
			OrderID = order.ID,
			Provider = PaymentProvider.VNPay,
			Status = PaymentStatus.Pending,
			Amount = 300m,
			CurrencyCode = "VND",
			ReferenceCode = $"PAY-{order.ID}-test",
		};
		await ctx.Payments.AddAsync(payment);
		await ctx.SaveChangesAsync(default);

		var result = await service.MarkSucceededAsync(
			payment.ID,
			new MarkPaymentSucceededCommand(null)
		);

		Assert.True(result.TryPickError(out var error));
		Assert.IsType<ValidationError>(error);
	}
}
