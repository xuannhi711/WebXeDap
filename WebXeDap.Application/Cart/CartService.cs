using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Common.Exceptions;
using WebXeDap.Application.Common.Extensions;
using WebXeDap.Application.Common.Interfaces;
using WebXeDap.Application.Cart.Models;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Cart;

public sealed class CartService
{
	private readonly IApplicationDbContext _context;
	private readonly ICurrentUserService _currentUser;

	public CartService(IApplicationDbContext context, ICurrentUserService currentUser)
	{
		_context = context;
		_currentUser = currentUser;
	}

	public async Task<CartDto> GetCartAsync(CancellationToken cancellationToken)
	{
		var userId = _currentUser.GetRequiredUserId();

		var cartItems = await _context.CartItems
			.AsNoTracking()
			.Include(ci => ci.Product)
			.ThenInclude(p => p.Images)
			.Where(ci => ci.UserID == userId)
			.ToListAsync(cancellationToken);

		var items = cartItems.Select(ci =>
		{
			var imageKey = ci.Product.Images
				.OrderBy(i => i.Order)
				.Select(i => i.key)
				.FirstOrDefault();

			return new CartItemDto
			{
				ProductId = ci.ProductID,
				ProductName = ci.Product.Name,
				Quantity = ci.Quantity,
				UnitPrice = ci.Product.Price,
				CurrencySymbol = ci.Product.CurrencySymbol,
				LineTotal = ci.Product.Price * ci.Quantity,
				ImageKey = imageKey
			};
		}).ToList();

		var subTotal = items.Sum(i => i.LineTotal);
		var currencySymbol = items.FirstOrDefault()?.CurrencySymbol ?? "VNĐ";

		return new CartDto
		{
			Items = items,
			SubTotal = subTotal,
			CurrencySymbol = currencySymbol
		};
	}

	public async Task<int> AddToCartAsync(
		int productId,
		int quantity,
		CancellationToken cancellationToken
	)
	{
		var userId = _currentUser.GetRequiredUserId();

		if (quantity <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
		}

		var product = await _context.Products
			.FirstOrDefaultAsync(p => p.ID == productId && !p.IsDeleted, cancellationToken);

		if (product is null)
		{
			throw new NotFoundException(nameof(Product), productId);
		}

		var cartItem = await _context.CartItems
			.FirstOrDefaultAsync(
				ci => ci.UserID == userId && ci.ProductID == productId,
				cancellationToken
			);

		if (cartItem is not null)
		{
			var newQuantity = cartItem.Quantity + quantity;
			if (product.Quantity < newQuantity)
			{
				throw new InvalidOperationException("Not enough stock for this product.");
			}

			cartItem.Quantity = newQuantity;
			await _context.SaveChangesAsync(cancellationToken);
			return cartItem.ID;
		}

		if (product.Quantity < quantity)
		{
			throw new InvalidOperationException("Not enough stock for this product.");
		}

		var user = await _context.Users
			.FirstOrDefaultAsync(u => u.ID == userId, cancellationToken);

		if (user is null)
		{
			throw new NotFoundException(nameof(User), userId);
		}

		cartItem = new CartItem
		{
			UserID = userId,
			User = user,
			ProductID = product.ID,
			Product = product,
			Quantity = quantity
		};

		_context.CartItems.Add(cartItem);
		await _context.SaveChangesAsync(cancellationToken);

		return cartItem.ID;
	}

	public async Task UpdateCartItemAsync(
		int productId,
		int quantity,
		CancellationToken cancellationToken
	)
	{
		var userId = _currentUser.GetRequiredUserId();

		if (quantity < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");
		}

		var cartItem = await _context.CartItems
			.Include(ci => ci.Product)
			.FirstOrDefaultAsync(
				ci => ci.UserID == userId && ci.ProductID == productId,
				cancellationToken
			);

		if (cartItem is null)
		{
			throw new NotFoundException(nameof(CartItem), productId);
		}

		if (quantity == 0)
		{
			_context.CartItems.Remove(cartItem);
			await _context.SaveChangesAsync(cancellationToken);
			return;
		}

		if (cartItem.Product.IsDeleted)
		{
			throw new NotFoundException(nameof(Product), productId);
		}

		if (cartItem.Product.Quantity < quantity)
		{
			throw new InvalidOperationException("Not enough stock for this product.");
		}

		cartItem.Quantity = quantity;
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task RemoveCartItemAsync(int productId, CancellationToken cancellationToken)
	{
		var userId = _currentUser.GetRequiredUserId();

		var cartItem = await _context.CartItems
			.FirstOrDefaultAsync(
				ci => ci.UserID == userId && ci.ProductID == productId,
				cancellationToken
			);

		if (cartItem is null)
		{
			throw new NotFoundException(nameof(CartItem), productId);
		}

		_context.CartItems.Remove(cartItem);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task ClearCartAsync(CancellationToken cancellationToken)
	{
		var userId = _currentUser.GetRequiredUserId();

		var cartItems = await _context.CartItems
			.Where(ci => ci.UserID == userId)
			.ToListAsync(cancellationToken);

		if (cartItems.Count == 0)
		{
			return;
		}

		_context.CartItems.RemoveRange(cartItems);
		await _context.SaveChangesAsync(cancellationToken);
	}
}
