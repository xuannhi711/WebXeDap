namespace WebXeDap.Application.Features.Catalog.DTOs;

public record CreateProductCommand(
	string Name,
	string? Description,
	decimal Price,
	int Quantity,
	ICollection<int>? CategoryIDs,
	string CurrencySymbol = "VNĐ"
);

public record UpdateProductCommand(
	string? Name,
	string? Description,
	decimal? Price,
	int? Quantity,
	ICollection<int>? CategoryIDs,
	string? CurrencySymbol
);

public record FilterProductCommand(
	string? Keyword,
	ICollection<int>? CategoryIDs,
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
	string CurrencySymbol,
	int Quantity,
	ICollection<CategoryResponse> Categories,
	ICollection<ProductImageResponse> Images,
	DateTime CreatedAt,
	DateTime? UpdatedAt,
	bool IsDeleted,
	DateTime? DeletedAt
);
