using WebXeDap.Domain.Enums;

namespace WebXeDap.Domain.Models;

public class Payment : BaseAuditableEntity
{
	public int ID { get; set; }
	public int OrderID { get; set; }
	public Order Order { get; set; } = null!;
	public PaymentProvider Provider { get; set; }
	public PaymentStatus Status { get; set; }
	public decimal Amount { get; set; }
	public string CurrencyCode { get; set; } = "VND";
	public string ReferenceCode { get; set; } = string.Empty;
	public string? ProviderTransactionID { get; set; }
	public string? ProviderPaymentUrl { get; set; }
	public string? FailureReason { get; set; }
	public string? RawResponse { get; set; }
	public DateTime? CompletedAt { get; set; }

	public void MarkProcessing(string? providerPaymentUrl = null)
	{
		Status = PaymentStatus.Processing;
		ProviderPaymentUrl = providerPaymentUrl;
		SetUpdated();
	}

	public void MarkSucceeded(string? providerTransactionId = null, string? rawResponse = null)
	{
		Status = PaymentStatus.Succeeded;
		ProviderTransactionID = providerTransactionId;
		RawResponse = rawResponse;
		FailureReason = null;
		CompletedAt = DateTime.UtcNow;
		SetUpdated();
	}

	public void MarkFailed(string failureReason, string? rawResponse = null)
	{
		Status = PaymentStatus.Failed;
		FailureReason = failureReason;
		RawResponse = rawResponse;
		CompletedAt = DateTime.UtcNow;
		SetUpdated();
	}

	public void Cancel(string? rawResponse = null)
	{
		Status = PaymentStatus.Cancelled;
		RawResponse = rawResponse;
		CompletedAt = DateTime.UtcNow;
		SetUpdated();
	}
}
