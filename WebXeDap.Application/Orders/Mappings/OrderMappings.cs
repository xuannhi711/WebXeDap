using WebXeDap.Application.Orders.Models;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Orders.Mappings;

internal static class OrderMappings
{
	public static OrderDto ToDto(this Order order)
	{
		var items = order
			.OrderItems.Select(item => new OrderItemDto
			{
				ProductId = item.ProductID,
				ProductName = item.Product.Name,
				Quantity = item.Quantity,
				UnitPrice = item.UnitPrice,
				CurrencySymbol = item.Product.CurrencySymbol,
				LineTotal = item.UnitPrice * item.Quantity,
			})
			.ToList();

		return new OrderDto
		{
			Id = order.ID,
			OrderDate = order.OrderDate,
			SubTotal = order.SubTotal,
			TotalAmount = order.TotalAmount,
			Items = items,
		};
	}
}
