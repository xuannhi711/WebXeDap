namespace WebXeDap.Application.Cart.Models;

public sealed record CartItemDto
{
	public int ProductId { get; init; }
	public string ProductName { get; init; } = string.Empty;
	public int Quantity { get; init; }
	public decimal UnitPrice { get; init; }
	public string CurrencySymbol { get; init; } = "VNĐ";
	public decimal LineTotal { get; init; }
	public string? ImageKey { get; init; }
}
