using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Common.Exceptions;
using WebXeDap.Application.Common.Extensions;
using WebXeDap.Application.Common.Interfaces;
using WebXeDap.Application.Orders.Mappings;
using WebXeDap.Application.Orders.Models;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Orders;

public sealed class OrderService
{
	private readonly IApplicationDbContext _context;
	private readonly ICurrentUserService _currentUser;

	public OrderService(IApplicationDbContext context, ICurrentUserService currentUser)
	{
		_context = context;
		_currentUser = currentUser;
	}

	public async Task<int> CreateOrderAsync(CancellationToken cancellationToken)
	{
		var userId = _currentUser.GetRequiredUserId();

		var user = await _context.Users
			.FirstOrDefaultAsync(u => u.ID == userId, cancellationToken);

		if (user is null)
		{
			throw new NotFoundException(nameof(User), userId);
		}

		var cartItems = await _context.CartItems
			.Include(ci => ci.Product)
			.Where(ci => ci.UserID == userId)
			.ToListAsync(cancellationToken);

		if (cartItems.Count == 0)
		{
			throw new InvalidOperationException("Cart is empty.");
		}

		var order = new Order
		{
			UserID = userId,
			User = user,
			OrderDate = DateTime.UtcNow
		};

		decimal subTotal = 0;
		foreach (var item in cartItems)
		{
			if (item.Product.IsDeleted)
			{
				throw new NotFoundException(nameof(Product), item.ProductID);
			}

			if (item.Product.Quantity < item.Quantity)
			{
				throw new InvalidOperationException("Not enough stock for this product.");
			}

			var unitPrice = item.Product.Price;
			subTotal += unitPrice * item.Quantity;

			order.OrderItems.Add(new OrderItem
			{
				ProductID = item.ProductID,
				Product = item.Product,
				Order = order,
				Quantity = item.Quantity,
				UnitPrice = unitPrice
			});

			item.Product.Quantity -= item.Quantity;
			item.Product.SetUpdated(userId);
		}

		order.SubTotal = subTotal;
		order.TotalAmount = subTotal;

		_context.Orders.Add(order);
		_context.CartItems.RemoveRange(cartItems);
		await _context.SaveChangesAsync(cancellationToken);

		return order.ID;
	}

	public async Task<IReadOnlyList<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken)
	{
		var userId = _currentUser.GetRequiredUserId();

		var orders = await _context.Orders
			.AsNoTracking()
			.Include(o => o.OrderItems)
			.ThenInclude(oi => oi.Product)
			.Where(o => o.UserID == userId)
			.OrderByDescending(o => o.OrderDate)
			.ToListAsync(cancellationToken);

		return orders.Select(o => o.ToDto()).ToList();
	}

	public async Task<OrderDto> GetOrderByIdAsync(int id, CancellationToken cancellationToken)
	{
		var userId = _currentUser.GetRequiredUserId();

		var order = await _context.Orders
			.AsNoTracking()
			.Include(o => o.OrderItems)
			.ThenInclude(oi => oi.Product)
			.FirstOrDefaultAsync(o => o.ID == id, cancellationToken);

		if (order is null)
		{
			throw new NotFoundException(nameof(Order), id);
		}

		if (order.UserID != userId)
		{
			throw new ForbiddenAccessException();
		}

		return order.ToDto();
	}
}
