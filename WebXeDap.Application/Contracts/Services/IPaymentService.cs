using Util.Primitives.ResultType;
using WebXeDap.Application.Features.Payments.DTOs;

namespace WebXeDap.Application.Contracts.Services;

public interface IPaymentService
{
	Task<Result<PaymentResponse>> GetByIDAsync(int id);

	Task<Result<PaymentResponse>> CreateAsync(CreatePaymentCommand cmd);

	Task<Result<PaymentResponse>> CheckoutAsync(CheckoutPaymentCommand cmd);

	Task<Result<PaymentResponse>> SetProcessingAsync(int id, SetPaymentProcessingCommand cmd);

	Task<Result<PaymentResponse>> MarkSucceededAsync(int id, MarkPaymentSucceededCommand cmd);

	Task<Result<PaymentResponse>> MarkFailedAsync(int id, MarkPaymentFailedCommand cmd);
	Task<Result<PaymentResponse>> CancelAsync(int id, CancelPaymentCommand cmd);
}
