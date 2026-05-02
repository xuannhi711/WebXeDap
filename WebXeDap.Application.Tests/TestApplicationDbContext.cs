using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Common.Interfaces;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Tests;

public sealed class TestApplicationDbContext : DbContext, IApplicationDbContext
{
	public TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options)
		: base(options)
	{
	}

	public DbSet<Product> Products => Set<Product>();
	public DbSet<Category> Categories => Set<Category>();
	public DbSet<ProductImage> ProductImages => Set<ProductImage>();
	public DbSet<CartItem> CartItems => Set<CartItem>();
	public DbSet<Order> Orders => Set<Order>();
	public DbSet<OrderItem> OrderItems => Set<OrderItem>();
	public DbSet<User> Users => Set<User>();
	public DbSet<Notification> Notifications => Set<Notification>();
	public DbSet<Config> Configs => Set<Config>();

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
	{
		return base.SaveChangesAsync(cancellationToken);
	}
}
