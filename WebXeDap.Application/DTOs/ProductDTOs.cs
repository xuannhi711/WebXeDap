namespace WebXeDap.Application.DTOs;

public record CreateProductRequest(
	string Name,
	string? Description,
	decimal Price,
	int Quantity,
	int[]? CategoryIDs,
	string CurrencySymbol = "VNĐ"
);

public record UpdateProductRequest(
	int ID,
	string Name,
	string? Description,
	decimal Price,
	int Quantity,
	int[]? CategoryIDs,
	string CurrencySymbol
);

public record FilterProductRequest(
	string? Keyword,
	int[]? CategoryIDs,
	decimal? MinPrice,
	decimal? MaxPrice,
	int Page = 1,
	int Size = 20,
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
