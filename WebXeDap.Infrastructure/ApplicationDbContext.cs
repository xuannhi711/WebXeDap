using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Domain.Models;

namespace WebXeDap.Infrastructure;

public sealed class ApplicationDbContext
	: IdentityDbContext<User, IdentityRole<int>, int>,
		IApplicationDbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options) { }

	public DbSet<Product> Products => Set<Product>();
	public DbSet<Category> Categories => Set<Category>();
	public DbSet<ProductImage> ProductImages => Set<ProductImage>();
	public DbSet<CartItem> CartItems => Set<CartItem>();
	public DbSet<Order> Orders => Set<Order>();
	public DbSet<OrderItem> OrderItems => Set<OrderItem>();
	public DbSet<Notification> Notifications => Set<Notification>();
	public DbSet<Config> Configs => Set<Config>();
	DbSet<User> IApplicationDbContext.Users => Users;

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
	{
		return base.SaveChangesAsync(cancellationToken);
	}
}
