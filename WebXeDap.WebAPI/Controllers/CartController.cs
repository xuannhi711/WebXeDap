using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Cart.DTOs;
using WebXeDap.WebAPI.Extensions;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/cart")]
public sealed class CartController : ControllerBase
{
	private readonly ICartService _cartService;

	public CartController(ICartService cartService)
	{
		_cartService = cartService;
	}

	[HttpGet]
	public async Task<ActionResult<List<CartItemResponse>>> Get()
	{
		var result = await _cartService.ListAsync();
		return result.Match(Ok, this.MatchErrorResult);
	}

	[HttpPost]
	public async Task<ActionResult<CartItemResponse>> Add([FromBody] AddCartItemCommand request)
	{
		var result = await _cartService.AddAsync(request);
		return result.Match(Ok, this.MatchErrorResult);
	}

	[HttpPatch("{cartItemID:int}")]
	public async Task<ActionResult<CartItemResponse>> Update(
		int cartItemID,
		[FromBody] UpdateCartItemCommand request
	)
	{
		var result = await _cartService.UpdateAsync(cartItemID, request);
		return result.Match(Ok, this.MatchErrorResult);
	}

	[HttpDelete("{cartItemID:int}")]
	public async Task<IActionResult> Remove(int cartItemID)
	{
		var result = await _cartService.DeleteAsync(cartItemID);
		return result.Match(_ => NoContent(), this.MatchErrorResult);
	}
}
