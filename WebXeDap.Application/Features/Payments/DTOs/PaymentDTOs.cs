using WebXeDap.Domain.Enums;

namespace WebXeDap.Application.Features.Payments.DTOs;

public sealed record CreatePaymentCommand(
	int OrderID,
	PaymentProvider Provider,
	decimal? Amount = null,
	string CurrencyCode = "VND"
);

public sealed record CheckoutPaymentCommand(
	ICollection<int> CartItemIDs,
	PaymentProvider Provider,
	string CurrencyCode = "VND"
);

public sealed record SetPaymentProcessingCommand(string? ProviderPaymentUrl);

public sealed record MarkPaymentSucceededCommand(
	string? ProviderTransactionID,
	string? RawResponse = null
);

public sealed record MarkPaymentFailedCommand(string FailureReason, string? RawResponse = null);

public sealed record CancelPaymentCommand(string? RawResponse = null);

public sealed record PaymentResponse
{
	public int ID { get; init; }
	public int OrderID { get; init; }
	public PaymentProvider Provider { get; init; }
	public PaymentStatus Status { get; init; }
	public decimal Amount { get; init; }
	public string CurrencyCode { get; init; } = string.Empty;
	public string ReferenceCode { get; init; } = string.Empty;
	public string? ProviderTransactionID { get; init; }
	public string? ProviderPaymentUrl { get; init; }
	public string? FailureReason { get; init; }
	public DateTime? CompletedAt { get; init; }
}
