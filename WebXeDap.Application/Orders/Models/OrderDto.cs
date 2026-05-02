namespace WebXeDap.Application.Orders.Models;

public sealed record OrderDto
{
	public int Id { get; init; }
	public DateTime OrderDate { get; init; }
	public decimal SubTotal { get; init; }
	public decimal TotalAmount { get; init; }
	public IReadOnlyList<OrderItemDto> Items { get; init; } = [];
}
