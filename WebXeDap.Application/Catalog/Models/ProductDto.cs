namespace WebXeDap.Application.Catalog.Models;

public sealed record ProductDto
{
	public int Id { get; init; }
	public string Name { get; init; } = string.Empty;
	public string? Description { get; init; }
	public decimal Price { get; init; }
	public string CurrencySymbol { get; init; } = "VNĐ";
	public int Quantity { get; init; }
	public IReadOnlyList<CategoryDto> Categories { get; init; } = [];
	public IReadOnlyList<ProductImageDto> Images { get; init; } = [];
}
