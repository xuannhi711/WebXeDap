using WebXeDap.Application.Extensions;

namespace WebXeDap.Application.DTOs;

public record CreateProductRequest(
	string Name,
	string? Description,
	decimal Price,
	int Quantity,
	int[]? CategoryIDs,
	string? CurrencySymbol = "VNĐ"
);

public record UpdateProductRequest(
	int ID,
	string? Name,
	string? Description,
	decimal? Price,
	int? Quantity,
	int[]? CategoryIDs,
	string? CurrencySymbol
);

public record FilterProductRequest(
	string? Keyword,
	int[]? CategoryIDs,
	decimal? MinPrice,
	decimal? MaxPrice,
	string? SortBy,
	int Page = 1,
	int Size = 20,
	Order Order = Order.ASCENDING
);

public record ProductResponse(
	int ID,
	string Name,
	string Description,
	decimal Price,
	int Quantity,
	List<CategoryResponse> Categories,
	List<ProductImageResponse> Images,
	string CurrencySymbol
);
