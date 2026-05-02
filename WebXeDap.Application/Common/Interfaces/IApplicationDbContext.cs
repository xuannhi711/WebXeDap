using Microsoft.EntityFrameworkCore;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Common.Interfaces;

public interface IApplicationDbContext
{
	DbSet<Product> Products { get; }
	DbSet<Category> Categories { get; }
	DbSet<ProductImage> ProductImages { get; }
	DbSet<CartItem> CartItems { get; }
	DbSet<Order> Orders { get; }
	DbSet<OrderItem> OrderItems { get; }
	DbSet<User> Users { get; }
	DbSet<Notification> Notifications { get; }
	DbSet<Config> Configs { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
