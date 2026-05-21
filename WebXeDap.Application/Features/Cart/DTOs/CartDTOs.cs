using WebXeDap.Application.Features.Catalog.DTOs;

namespace WebXeDap.Application.Features.Cart.DTOs;

public record AddCartItemCommand(int ProductID, int Quantity);

public record UpdateCartItemCommand(int CartItemID, int? Quantity);

public record CartItemResponse(int ID, SimpleProductResponse Product, int Quantity);
