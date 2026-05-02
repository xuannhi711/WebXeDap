namespace WebXeDap.Application.Orders.Models;

public sealed record OrderItemDto
{
	public int ProductId { get; init; }
	public string ProductName { get; init; } = string.Empty;
	public int Quantity { get; init; }
	public decimal UnitPrice { get; init; }
	public string CurrencySymbol { get; init; } = "VNĐ";
	public decimal LineTotal { get; init; }
}
