using Util.Primitives.ResultType;
using WebXeDap.Application.Features.Cart.DTOs;

namespace WebXeDap.Application.Contracts.Services;

public interface ICartService
{
	Task<Result<List<CartItemResponse>>> ListAsync();

	Task<Result<CartItemResponse>> AddAsync(AddCartItemCommand cmd);

	Task<Result<int>> CountAsync();

	Task<Result<CartItemResponse>> UpdateAsync(int cartItemID, UpdateCartItemCommand cmd);

	Task<Result> DeleteAsync(int cartItemID);
}
