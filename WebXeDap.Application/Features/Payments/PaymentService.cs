using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Extensions;
using WebXeDap.Application.Features.Payments.DTOs;
using WebXeDap.Application.Features.Payments.Mapper;
using WebXeDap.Application.Features.Payments.Validators;
using WebXeDap.Domain.Enums;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Payments;

public sealed class PaymentService : IPaymentService
{
	private readonly IApplicationDbContext ctx;
	private readonly ICurrentUserService currentUserService;
	private readonly PaymentMapper mapper;
	private readonly IValidator<CreatePaymentCommand> createValidator;
	private readonly IValidator<CheckoutPaymentCommand> checkoutValidator;
	private readonly IValidator<SetPaymentProcessingCommand> processingValidator;
	private readonly IValidator<MarkPaymentSucceededCommand> succeededValidator;
	private readonly IValidator<MarkPaymentFailedCommand> failedValidator;

	public PaymentService(
		IApplicationDbContext ctx,
		ICurrentUserService currentUserService,
		PaymentMapper mapper,
		IValidator<CreatePaymentCommand> createValidator,
		IValidator<CheckoutPaymentCommand> checkoutValidator,
		IValidator<SetPaymentProcessingCommand> processingValidator,
		IValidator<MarkPaymentSucceededCommand> succeededValidator,
		IValidator<MarkPaymentFailedCommand> failedValidator
	)
	{
		this.ctx = ctx;
		this.currentUserService = currentUserService;
		this.mapper = mapper;
		this.createValidator = createValidator;
		this.checkoutValidator = checkoutValidator;
		this.processingValidator = processingValidator;
		this.succeededValidator = succeededValidator;
		this.failedValidator = failedValidator;
	}

	public async Task<Result<PaymentResponse>> GetByIDAsync(int id)
	{
		var payment = await ctx
			.Payments.AsNoTracking()
			.FirstOrDefaultAsync(p => p.ID == id, default);
		if (payment is null)
		{
			return new NotFoundError("Payment not found.");
		}

		return mapper.ToPaymentResponse(payment);
	}

	public async Task<Result<PaymentResponse>> CreateAsync(CreatePaymentCommand cmd)
	{
		var validationResult = await createValidator.ValidateAsync(cmd);
		if (!validationResult.IsValid)
		{
			return validationResult.ToValidationError();
		}

		var order = await ctx
			.Orders.AsNoTracking()
			.FirstOrDefaultAsync(o => o.ID == cmd.OrderID, default);
		if (order is null)
		{
			return new NotFoundError("Order not found.");
		}

		var amount = cmd.Amount ?? order.TotalAmount;
		if (amount <= 0)
		{
			return new UnknownError("Payment amount must be greater than zero.");
		}

		var payment = new Payment
		{
			OrderID = order.ID,
			Provider = cmd.Provider,
			Status = PaymentStatus.Pending,
			Amount = amount,
			CurrencyCode = NormalizeCurrency(cmd.CurrencyCode),
			ReferenceCode = BuildReferenceCode(order.ID),
		};

		await ctx.Payments.AddAsync(payment, default);
		var res = await ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return new UnknownError("Failed to create payment.");
		}

		return mapper.ToPaymentResponse(payment);
	}

	public async Task<Result<PaymentResponse>> CheckoutAsync(CheckoutPaymentCommand cmd)
	{
		if (!currentUserService.UserID.TryPickValue(out var userID))
		{
			return new UnauthorizedError("User is not authenticated.");
		}

		var validationResult = await checkoutValidator.ValidateAsync(cmd);
		if (!validationResult.IsValid)
		{
			return validationResult.ToValidationError();
		}

		var cartItems = await ctx
			.CartItems.Include(i => i.Product)
			.Where(i => i.UserID == userID && cmd.CartItemIDs.Contains(i.ID))
			.ToListAsync(default);

		if (cartItems.Count != cmd.CartItemIDs.Count)
		{
			return new NotFoundError("One or more cart items were not found.");
		}

		var totalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity);
		if (totalAmount <= 0)
		{
			return new UnknownError("Checkout total must be greater than zero.");
		}

		var order = new Order
		{
			UserID = userID,
			OrderDate = DateTime.UtcNow,
			SubTotal = totalAmount,
			TotalAmount = totalAmount,
		};

		await ctx.Orders.AddAsync(order, default);
		var orderSaveResult = await ctx.SaveChangesAsync(default);
		if (orderSaveResult == 0)
		{
			return new UnknownError("Failed to create order.");
		}

		var orderItems = cartItems
			.Select(item => new OrderItem
			{
				OrderID = order.ID,
				ProductID = item.ProductID,
				Quantity = item.Quantity,
				UnitPrice = item.Product.Price,
			})
			.ToList();

		await ctx.OrderItems.AddRangeAsync(orderItems, default);

		var payment = new Payment
		{
			OrderID = order.ID,
			Provider = cmd.Provider,
			Status = PaymentStatus.Pending,
			Amount = totalAmount,
			CurrencyCode = NormalizeCurrency(cmd.CurrencyCode),
			ReferenceCode = BuildReferenceCode(order.ID),
		};

		await ctx.Payments.AddAsync(payment, default);
		var saveResult = await ctx.SaveChangesAsync(default);
		if (saveResult == 0)
		{
			return new UnknownError("Failed to create checkout payment.");
		}

		return mapper.ToPaymentResponse(payment);
	}

	public async Task<Result<PaymentResponse>> SetProcessingAsync(
		int id,
		SetPaymentProcessingCommand cmd
	)
	{
		var payment = await ctx.Payments.FirstOrDefaultAsync(p => p.ID == id, default);
		if (payment is null)
		{
			return new NotFoundError("Payment not found.");
		}

		var validationResult = await processingValidator.ValidateAsync(cmd);
		if (!validationResult.IsValid)
		{
			return validationResult.ToValidationError();
		}

		payment.MarkProcessing(cmd.ProviderPaymentUrl);
		ctx.Payments.Update(payment);
		var res = await ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return new UnknownError("Failed to update payment.");
		}

		return mapper.ToPaymentResponse(payment);
	}

	public async Task<Result<PaymentResponse>> MarkSucceededAsync(
		int id,
		MarkPaymentSucceededCommand cmd
	)
	{
		var payment = await ctx.Payments.FirstOrDefaultAsync(p => p.ID == id, default);
		if (payment is null)
		{
			return new NotFoundError("Payment not found.");
		}

		var validationResult = await succeededValidator.ValidateAsync(cmd);
		if (!validationResult.IsValid)
		{
			return validationResult.ToValidationError();
		}

		payment.MarkSucceeded(cmd.ProviderTransactionID, cmd.RawResponse);
		ctx.Payments.Update(payment);
		var res = await ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return new UnknownError("Failed to update payment.");
		}

		return mapper.ToPaymentResponse(payment);
	}

	public async Task<Result<PaymentResponse>> MarkFailedAsync(int id, MarkPaymentFailedCommand cmd)
	{
		var payment = await ctx.Payments.FirstOrDefaultAsync(p => p.ID == id, default);
		if (payment is null)
		{
			return new NotFoundError("Payment not found.");
		}

		var validationResult = await failedValidator.ValidateAsync(cmd);
		if (!validationResult.IsValid)
		{
			return validationResult.ToValidationError();
		}

		payment.MarkFailed(cmd.FailureReason, cmd.RawResponse);
		ctx.Payments.Update(payment);
		var res = await ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return new UnknownError("Failed to update payment.");
		}

		return mapper.ToPaymentResponse(payment);
	}

	public async Task<Result<PaymentResponse>> CancelAsync(int id, CancelPaymentCommand cmd)
	{
		var payment = await ctx.Payments.FirstOrDefaultAsync(p => p.ID == id, default);
		if (payment is null)
		{
			return new NotFoundError("Payment not found.");
		}

		payment.Cancel(cmd.RawResponse);
		ctx.Payments.Update(payment);
		var res = await ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return new UnknownError("Failed to update payment.");
		}

		return mapper.ToPaymentResponse(payment);
	}

	private static string BuildReferenceCode(int orderID)
	{
		return $"PAY-{orderID}-{Guid.NewGuid():N}";
	}

	private static string NormalizeCurrency(string currencyCode)
	{
		return string.IsNullOrWhiteSpace(currencyCode)
			? "VND"
			: currencyCode.Trim().ToUpperInvariant();
	}
}
