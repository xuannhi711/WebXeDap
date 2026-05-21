using Microsoft.AspNetCore.Mvc;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Cart.DTOs;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.WebAPI.Controllers;

namespace WebXeDap.WebAPI.UnitTests.Controllers;

[Trait("Category", "Unit")]
public class CartControllerTests
{
	[Fact]
	public async Task Get_ReturnsUnauthorized_WhenNoUser()
	{
		var controller = new CartController(
			new FixedCartService(Result<List<CartItemResponse>>.Ok([])),
			new FixedCurrentUserService { UserID = new NotFoundError("User not found.") }
		);

		var result = await controller.Get();

		Assert.IsType<UnauthorizedResult>(result.Result);
	}

	[Fact]
	public async Task Add_ReturnsBadRequest_WhenValidationError()
	{
		Result<CartItemResponse> invalidResult = new ValidationError(
			new Dictionary<string, string[]> { ["quantity"] = ["Invalid"] }
		);
		var controller = new CartController(
			new FixedCartService(invalidResult),
			new FixedCurrentUserService { UserID = 1 }
		);

		var result = await controller.Add(new CartController.AddCartItemRequest(1, 1));

		var response = Assert.IsType<BadRequestObjectResult>(result.Result);
		Assert.Equal(400, response.StatusCode);
	}

	[Fact]
	public async Task Get_ReturnsOk_WithCart()
	{
		var product = new SimpleProductResponse(
			ID: 5,
			Name: "Bike",
			Description: null,
			Price: 120m,
			CurrencySymbol: "VND",
			Quantity: 10,
			Image: null,
			CreatedAt: DateTime.UtcNow,
			UpdatedAt: null,
			IsDeleted: false,
			DeletedAt: null
		);
		var cart = new List<CartItemResponse> { new(1, product, 1) };
		var controller = new CartController(
			new FixedCartService(cart),
			new FixedCurrentUserService { UserID = 7 }
		);

		var result = await controller.Get();

		var ok = Assert.IsType<OkObjectResult>(result.Result);
		var payload = Assert.IsType<List<CartItemResponse>>(ok.Value);
		Assert.Single(payload);
	}

	[Fact]
	public async Task Update_ReturnsUnauthorized_WhenNoUser()
	{
		var controller = new CartController(
			new FixedCartService(Result<CartItemResponse>.Ok(default!)),
			new FixedCurrentUserService { UserID = new NotFoundError("User not found.") }
		);

		var result = await controller.Update(1, new CartController.UpdateCartItemRequest(2));

		Assert.IsType<UnauthorizedResult>(result.Result);
	}

	[Fact]
	public async Task Update_ReturnsOk_WhenSuccess()
	{
		var product = new SimpleProductResponse(
			ID: 5,
			Name: "Bike",
			Description: null,
			Price: 120m,
			CurrencySymbol: "VND",
			Quantity: 10,
			Image: null,
			CreatedAt: DateTime.UtcNow,
			UpdatedAt: null,
			IsDeleted: false,
			DeletedAt: null
		);
		var item = new CartItemResponse(1, product, 2);
		var controller = new CartController(
			new FixedCartService(Result<CartItemResponse>.Ok(item)),
			new FixedCurrentUserService { UserID = 7 }
		);

		var result = await controller.Update(1, new CartController.UpdateCartItemRequest(2));

		var ok = Assert.IsType<OkObjectResult>(result.Result);
		Assert.IsType<CartItemResponse>(ok.Value);
	}

	[Fact]
	public async Task Remove_ReturnsUnauthorized_WhenNoUser()
	{
		var controller = new CartController(
			new FixedCartService(Result.Ok()),
			new FixedCurrentUserService { UserID = new NotFoundError("User not found.") }
		);

		var result = await controller.Remove(1);

		Assert.IsType<UnauthorizedResult>(result);
	}

	[Fact]
	public async Task Remove_ReturnsNoContent_WhenSuccess()
	{
		var controller = new CartController(
			new FixedCartService(Result.Ok()),
			new FixedCurrentUserService { UserID = 7 }
		);

		var result = await controller.Remove(1);

		Assert.IsType<NoContentResult>(result);
	}

	private sealed class FixedCartService : ICartService
	{
		private readonly Result<List<CartItemResponse>> _listResult;
		private readonly Result<CartItemResponse> _itemResult;
		private readonly Result _deleteResult;

		public FixedCartService(Result<List<CartItemResponse>> result)
		{
			_listResult = result;
			_itemResult = new NotFoundError("Not configured.");
			_deleteResult = new NotFoundError("Not configured.");
		}

		public FixedCartService(Result<CartItemResponse> result)
		{
			_itemResult = result;
			_listResult = new NotFoundError("Not configured.");
			_deleteResult = new NotFoundError("Not configured.");
		}

		public FixedCartService(Result deleteResult)
		{
			_deleteResult = deleteResult;
			_itemResult = new NotFoundError("Not configured.");
			_listResult = new NotFoundError("Not configured.");
		}

		public Task<Result<List<CartItemResponse>>> ListAsync()
		{
			return Task.FromResult(_listResult);
		}

		public Task<Result<CartItemResponse>> AddAsync(AddCartItemCommand cmd)
		{
			return Task.FromResult(_itemResult);
		}

		public Task<Result<CartItemResponse>> UpdateAsync(UpdateCartItemCommand cmd)
		{
			return Task.FromResult(_itemResult);
		}

		public Task<Result> DeleteAsync(int cartItemID)
		{
			return Task.FromResult(_deleteResult);
		}
	}

	private sealed class FixedCurrentUserService : ICurrentUserService
	{
		public Result<int> UserID { get; set; }
	}
}
