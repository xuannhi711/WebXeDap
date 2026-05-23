using FluentValidation.TestHelper;
using WebXeDap.Application.Features.Payments.DTOs;
using WebXeDap.Application.Features.Payments.Validators;
using WebXeDap.Domain.Enums;

namespace WebXeDap.Application.UnitTests.Payments;

[Trait("Category", "Unit")]
public sealed class CreatePaymentValidatorTests
{
	private readonly CreatePaymentValidator validator = new();

	[Fact]
	public async Task CreatePaymentValidator_Pass_WhenRequestIsValid()
	{
		var req = new CreatePaymentCommand(1, PaymentProvider.Manual, 120m, "vnd");

		var result = await validator.TestValidateAsync(req);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task CreatePaymentValidator_Fail_WhenRequestIsInvalid()
	{
		var req = new CreatePaymentCommand(0, PaymentProvider.Manual, 0m, "");

		var result = await validator.TestValidateAsync(req);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(r => r.OrderID);
		result.ShouldHaveValidationErrorFor(r => r.Amount);
		result.ShouldHaveValidationErrorFor(r => r.CurrencyCode);
	}
}

[Trait("Category", "Unit")]
public sealed class CheckoutPaymentValidatorTests
{
	private readonly CheckoutPaymentValidator validator = new();

	[Fact]
	public async Task CheckoutPaymentValidator_Pass_WhenRequestIsValid()
	{
		var req = new CheckoutPaymentCommand([1, 2], PaymentProvider.Manual, "VND");

		var result = await validator.TestValidateAsync(req);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task CheckoutPaymentValidator_Fail_WhenCartIsEmptyOrInvalid()
	{
		var req = new CheckoutPaymentCommand([0], PaymentProvider.Manual, "");

		var result = await validator.TestValidateAsync(req);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(r => r.CartItemIDs);
		result.ShouldHaveValidationErrorFor(r => r.CurrencyCode);
	}
}

[Trait("Category", "Unit")]
public sealed class SetPaymentProcessingValidatorTests
{
	private readonly SetPaymentProcessingValidator validator = new();

	[Fact]
	public async Task SetPaymentProcessingValidator_Pass_WhenRequestIsValid()
	{
		var req = new SetPaymentProcessingCommand("https://example.com/pay");

		var result = await validator.TestValidateAsync(req);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task SetPaymentProcessingValidator_Fail_WhenUrlMissing()
	{
		var req = new SetPaymentProcessingCommand(null);

		var result = await validator.TestValidateAsync(req);

		result.ShouldHaveValidationErrorFor(r => r.ProviderPaymentUrl);
	}
}

[Trait("Category", "Unit")]
public sealed class MarkPaymentSucceededValidatorTests
{
	private readonly MarkPaymentSucceededValidator validator = new();

	[Fact]
	public async Task MarkPaymentSucceededValidator_Pass_WhenRequestIsValid()
	{
		var req = new MarkPaymentSucceededCommand("TXN-001");

		var result = await validator.TestValidateAsync(req);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task MarkPaymentSucceededValidator_Fail_WhenTransactionIdMissing()
	{
		var req = new MarkPaymentSucceededCommand(null);

		var result = await validator.TestValidateAsync(req);

		result.ShouldHaveValidationErrorFor(r => r.ProviderTransactionID);
	}
}

[Trait("Category", "Unit")]
public sealed class MarkPaymentFailedValidatorTests
{
	private readonly MarkPaymentFailedValidator validator = new();

	[Fact]
	public async Task MarkPaymentFailedValidator_Pass_WhenRequestIsValid()
	{
		var req = new MarkPaymentFailedCommand("Declined");

		var result = await validator.TestValidateAsync(req);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task MarkPaymentFailedValidator_Fail_WhenReasonMissing()
	{
		var req = new MarkPaymentFailedCommand("");

		var result = await validator.TestValidateAsync(req);

		result.ShouldHaveValidationErrorFor(r => r.FailureReason);
	}
}
