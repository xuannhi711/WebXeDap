using Riok.Mapperly.Abstractions;
using WebXeDap.Application.Features.Cart.DTOs;
using WebXeDap.Application.Features.Catalog.Mapper;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Cart.Mapper;

[Mapper(AllowNullPropertyAssignment = false)]
public partial class CartMapper
{
	[UseMapper]
	private readonly ProductMapper productMapper;

	public CartMapper(ProductMapper productMapper)
	{
		this.productMapper = productMapper;
	}

	[MapperIgnoreSource(nameof(CartItem.UserID))]
	[MapperIgnoreSource(nameof(CartItem.User))]
	[MapperIgnoreSource(nameof(CartItem.ProductID))]
	public partial CartItemResponse ToCartItemResponse(CartItem item);

	public partial List<CartItemResponse> ToCartItemResponseList(List<CartItem> items);

	[MapperIgnoreTarget(nameof(CartItem.ID))]
	[MapperIgnoreTarget(nameof(CartItem.User))]
	[MapperIgnoreTarget(nameof(CartItem.Product))]
	public partial CartItem ToCartItem(AddCartItemCommand cmd, int userID);

	[MapperIgnoreTarget(nameof(CartItem.User))]
	[MapperIgnoreTarget(nameof(CartItem.Product))]
	[MapperIgnoreTarget(nameof(CartItem.UserID))]
	[MapperIgnoreTarget(nameof(CartItem.ProductID))]
	[MapProperty(nameof(UpdateCartItemCommand.CartItemID), nameof(CartItem.ID))]
	public partial void PatchCartItem(UpdateCartItemCommand cmd, [MappingTarget] CartItem item);
}
