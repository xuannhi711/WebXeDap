namespace WebXeDap.Application.Cart.Models;

public sealed record CartDto
{
	public IReadOnlyList<CartItemDto> Items { get; init; } = [];
	public decimal SubTotal { get; init; }
	public string CurrencySymbol { get; init; } = "VNĐ";
}
