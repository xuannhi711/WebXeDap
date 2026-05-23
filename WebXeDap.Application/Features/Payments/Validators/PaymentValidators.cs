using FluentValidation;
using WebXeDap.Application.Features.Payments.DTOs;

namespace WebXeDap.Application.Features.Payments.Validators;

public sealed class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
{
	public CreatePaymentValidator()
	{
		RuleFor(cmd => cmd.OrderID).GreaterThan(0).WithMessage("Order id must be greater than 0.");
		RuleFor(cmd => cmd.Amount)
			.Must(amount => !amount.HasValue || amount.Value > 0)
			.WithMessage("Payment amount must be greater than 0.");
		RuleFor(cmd => cmd.CurrencyCode).NotEmpty().WithMessage("Currency code is required.");
	}
}

public sealed class CheckoutPaymentValidator : AbstractValidator<CheckoutPaymentCommand>
{
	public CheckoutPaymentValidator()
	{
		RuleFor(cmd => cmd.CartItemIDs)
			.NotEmpty()
			.WithMessage("At least one cart item must be selected.");
		RuleForEach(cmd => cmd.CartItemIDs)
			.GreaterThan(0)
			.WithMessage("Cart item id must be greater than 0.");
		RuleFor(cmd => cmd.CurrencyCode).NotEmpty().WithMessage("Currency code is required.");
	}
}

public sealed class SetPaymentProcessingValidator : AbstractValidator<SetPaymentProcessingCommand>
{
	public SetPaymentProcessingValidator()
	{
		RuleFor(cmd => cmd.ProviderPaymentUrl)
			.NotEmpty()
			.WithMessage("Provider payment url is required.");
	}
}

public sealed class MarkPaymentSucceededValidator : AbstractValidator<MarkPaymentSucceededCommand>
{
	public MarkPaymentSucceededValidator()
	{
		RuleFor(cmd => cmd.ProviderTransactionID)
			.NotEmpty()
			.WithMessage("Provider transaction id is required.");
	}
}

public sealed class MarkPaymentFailedValidator : AbstractValidator<MarkPaymentFailedCommand>
{
	public MarkPaymentFailedValidator()
	{
		RuleFor(cmd => cmd.FailureReason).NotEmpty().WithMessage("Failure reason is required.");
	}
}
