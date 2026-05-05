namespace WebXeDap.Application.DTOs;

public record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    int Quantity,
    List<int>? CategoryIDs,
    string? CurrencySymbol = "VNĐ"
);

public record UpdateProductRequest(
    int ID,
    string? Name,
    string? Description,
    decimal? Price,
    int? Quantity,
    List<int>? CategoryIDs,
    string? CurrencySymbol
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