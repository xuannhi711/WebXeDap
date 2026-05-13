namespace WebXeDap.Application.Features.Catalog.DTOs;

public record CreateProductCommand(
	string Name,
	string? Description,
	decimal Price,
	int Quantity,
	int[]? CategoryIDs,
	string CurrencySymbol = "VNĐ"
);

public record UpdateProductCommand(
	int ID,
	string Name,
	string? Description,
	decimal Price,
	int Quantity,
	int[]? CategoryIDs,
	string CurrencySymbol
);

public record FilterProductCommand(
	string? Keyword,
	int[]? CategoryIDs,
	decimal? MinPrice,
	decimal? MaxPrice,
	string SortBy = "id",
	bool IsAscending = true
);

public record SimpleProductResponse(
	int ID,
	string Name,
	string? Description,
	decimal Price,
	string CurrencySymbol,
	int Quantity,
	ProductImageResponse? Image,
	DateTime CreatedAt,
	DateTime? UpdatedAt,
	bool IsDeleted,
	DateTime? DeletedAt
);

public record DetailedProductResponse(
	int ID,
	string Name,
	string? Description,
	decimal Price,
	int Quantity,
	List<CategoryResponse> Categories,
	List<ProductImageResponse> Images,
	string CurrencySymbol,
	DateTime CreatedAt,
	DateTime? UpdatedAt,
	bool IsDeleted,
	DateTime? DeletedAt
);
