using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Payments.DTOs;
using WebXeDap.WebAPI.Extensions;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/payments")]
public sealed class PaymentsController : ControllerBase
{
	private readonly IPaymentService paymentService;

	public PaymentsController(IPaymentService paymentService)
	{
		this.paymentService = paymentService;
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<PaymentResponse>> GetByID(int id)
	{
		var result = await paymentService.GetByIDAsync(id);
		return result.Match(Ok, this.MatchErrorResult);
	}

	[HttpPost]
	public async Task<ActionResult<PaymentResponse>> Create([FromBody] CreatePaymentCommand request)
	{
		var result = await paymentService.CreateAsync(request);
		return result.Match(
			val => CreatedAtAction(nameof(GetByID), new { id = val.ID }, val),
			this.MatchErrorResult
		);
	}

	[HttpPost("checkout")]
	public async Task<ActionResult<PaymentResponse>> Checkout(
		[FromBody] CheckoutPaymentCommand request
	)
	{
		var result = await paymentService.CheckoutAsync(request);
		return result.Match(
			val => CreatedAtAction(nameof(GetByID), new { id = val.ID }, val),
			this.MatchErrorResult
		);
	}

	[HttpPost("{id:int}/processing")]
	public async Task<ActionResult<PaymentResponse>> SetProcessing(
		int id,
		[FromBody] SetPaymentProcessingCommand request
	)
	{
		var result = await paymentService.SetProcessingAsync(id, request);
		return result.Match(Ok, this.MatchErrorResult);
	}

	[HttpPost("{id:int}/succeeded")]
	[AllowAnonymous]
	public async Task<ActionResult<PaymentResponse>> MarkSucceeded(
		int id,
		[FromBody] MarkPaymentSucceededCommand request
	)
	{
		var result = await paymentService.MarkSucceededAsync(id, request);
		return result.Match(Ok, this.MatchErrorResult);
	}

	[HttpPost("{id:int}/failed")]
	[AllowAnonymous]
	public async Task<ActionResult<PaymentResponse>> MarkFailed(
		int id,
		[FromBody] MarkPaymentFailedCommand request
	)
	{
		var result = await paymentService.MarkFailedAsync(id, request);
		return result.Match(Ok, this.MatchErrorResult);
	}

	[HttpPost("{id:int}/cancel")]
	[AllowAnonymous]
	public async Task<ActionResult<PaymentResponse>> Cancel(
		int id,
		[FromBody] CancelPaymentCommand request
	)
	{
		var result = await paymentService.CancelAsync(id, request);
		return result.Match(Ok, this.MatchErrorResult);
	}
}
