using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Application.Contracts;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Cart.DTOs;
using WebXeDap.WebAPI.Extensions;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("cart")]
public sealed class CartController : ControllerBase
{
	private readonly ICartService _cartService;
	private readonly ICurrentUserService _currentUser;

	public CartController(ICartService cartService, ICurrentUserService currentUser)
	{
		_cartService = cartService;
		_currentUser = currentUser;
	}

	[HttpGet]
	public async Task<ActionResult<List<CartItemResponse>>> Get()
	{
		if (!TryGetUserId())
		{
			return Unauthorized();
		}

		var result = await _cartService.ListAsync();
		return this.ToActionResult(result);
	}

	[HttpPost]
	public async Task<ActionResult<CartItemResponse>> Add([FromBody] AddCartItemRequest request)
	{
		if (!TryGetUserId())
		{
			return Unauthorized();
		}

		var result = await _cartService.AddAsync(
			new AddCartItemCommand(request.ProductID, request.Quantity)
		);
		return this.ToActionResult(result);
	}

	[HttpPatch("{productId:int}")]
	public async Task<ActionResult<CartItemResponse>> Update(
		int productId,
		[FromBody] UpdateCartItemRequest request
	)
	{
		if (!TryGetUserId())
		{
			return Unauthorized();
		}

		var result = await _cartService.UpdateAsync(
			new UpdateCartItemCommand(productId, request.Quantity)
		);
		return this.ToActionResult(result);
	}

	[HttpDelete("{cartItemID:int}")]
	public async Task<IActionResult> Remove(int cartItemID)
	{
		if (!TryGetUserId())
		{
			return Unauthorized();
		}

		var result = await _cartService.DeleteAsync(cartItemID);
		return this.ToActionResult(result);
	}

	private bool TryGetUserId()
	{
		return _currentUser.UserID.TryPickValue(out _);
	}

	public sealed record AddCartItemRequest(
		[Range(1, int.MaxValue)] int ProductID,
		[Range(1, int.MaxValue)] int Quantity
	);

	public sealed record UpdateCartItemRequest([Range(1, int.MaxValue)] int Quantity);
}
