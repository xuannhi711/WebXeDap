using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Domain.Constants;
using WebXeDap.Domain.Enums;
using WebXeDap.Domain.Models;
using WebXeDap.Infrastructure;

namespace WebXeDap.Seeder;

public static class DbSeeder
{
	public static async Task SeedAsync(IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
		var roleManager = scope.ServiceProvider.GetRequiredService<
			RoleManager<IdentityRole<int>>
		>();

		await using var transaction = await context.Database.BeginTransactionAsync();

		// ROLES
		var roles = new[] { ROLES.ADMIN, ROLES.STAFF, ROLES.CUSTOMER };
		foreach (var role in roles)
		{
			if (!await roleManager.RoleExistsAsync(role))
			{
				await roleManager.CreateAsync(new IdentityRole<int>(role));
			}
		}

		// =====================================================
		// RANDOM USERS + RANDOM ROLES
		// =====================================================

		var userFaker = new Faker<User>()
			.RuleFor(x => x.FullName, f => f.Name.FullName())
			.RuleFor(x => x.Email, f => f.Internet.Email())
			.RuleFor(x => x.UserName, (_, u) => u.Email)
			.RuleFor(x => x.Avatar, f => $"https://i.pravatar.cc/150?img={f.Random.Int(1, 70)}")
			.RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber())
			.RuleFor(x => x.EmailConfirmed, true);

		var fakeUsers = userFaker.Generate(30);

		foreach (var user in fakeUsers)
		{
			var result = await userManager.CreateAsync(user, "Abc123!");

			if (!result.Succeeded)
			{
				continue;
			}

			// random role
			var randomRole = Random.Shared.GetItems(roles, 1).First();

			await userManager.AddToRoleAsync(user, randomRole);
		}

		// =====================================================
		// CATEGORIES
		// =====================================================

		var categories = new[]
		{
			new Category { Name = "Xe đạp địa hình" },
			new Category { Name = "Xe đạp đua" },
			new Category { Name = "Xe đạp touring" },
			new Category { Name = "Xe đạp trẻ em" },
			new Category { Name = "Phụ kiện" },
		};

		context.Categories.AddRange(categories);
		await context.SaveChangesAsync(default);

		// =====================================================
		// PRODUCTS
		// =====================================================

		var productFaker = new Faker<Product>()
			.RuleFor(x => x.Name, f => $"{f.Vehicle.Manufacturer()} {f.Vehicle.Model()}")
			.RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
			.RuleFor(x => x.Price, f => f.Random.Decimal(1_000_000, 50_000_000))
			.RuleFor(x => x.Quantity, f => f.Random.Int(0, 1000))
			.RuleFor(x => x.Categories, f => f.Random.ListItems(categories, f.Random.Int(1, 3)));

		var products = productFaker.Generate(120);

		context.Products.AddRange(products);
		await context.SaveChangesAsync(default);

		// =====================================================
		// PRODUCT IMAGES
		// =====================================================

		var productImages = new List<ProductImage>();

		foreach (var product in products)
		{
			var imageCount = Random.Shared.Next(2, 6);

			for (int i = 0; i < imageCount; i++)
			{
				productImages.Add(
					new ProductImage
					{
						ProductID = product.ID,
						Order = i + 1,
						Key = $"https://picsum.photos/500/500?random={product.ID * 10 + i}",
					}
				);
			}
		}

		context.ProductImages.AddRange(productImages);
		await context.SaveChangesAsync(default);
		// =====================================================
		// CART ITEMS
		// =====================================================
		await context.SaveChangesAsync(default);

		var users = await context.Users.ToListAsync();
		var cartItems = new List<CartItem>();

		foreach (var user in users)
		{
			// each user gets 1-10 cart items
			var itemCount = Random.Shared.Next(1, 11);

			// avoid duplicate products in same cart
			var randomProducts = Random
				.Shared.GetItems(products.ToArray(), Random.Shared.Next(3, 16))
				.Distinct()
				.ToList();

			foreach (var product in randomProducts)
			{
				cartItems.Add(
					new CartItem
					{
						UserID = user.Id,
						ProductID = product.ID,
						Quantity = Random.Shared.Next(1, 6),
					}
				);
			}
		}

		context.CartItems.AddRange(cartItems);
		await context.SaveChangesAsync(default);

		// =====================================================
		// SALE CAMPAIGNS
		// =====================================================

		var campaignFaker = new Faker<SaleCampaign>()
			.RuleFor(x => x.Name, f => $"{f.Commerce.ProductAdjective()} Sale")
			.RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
			.RuleFor(x => x.DiscountType, f => f.PickRandom<DiscountType>())
			.RuleFor(
				x => x.DiscountValue,
				(f, x) =>
					x.DiscountType == DiscountType.Percentage
						? f.Random.Decimal(5, 50)
						: f.Random.Decimal(50_000, 5_000_000)
			)
			.RuleFor(x => x.StartsAt, f => f.Date.Recent(15))
			.RuleFor(x => x.EndsAt, (f, x) => x.StartsAt.AddDays(f.Random.Int(7, 30)));

		var campaigns = campaignFaker.Generate(5);

		foreach (var campaign in campaigns)
		{
			// each campaign affects 3-15 random products
			var randomProducts = products
				.OrderBy(_ => Guid.NewGuid())
				.Take(Random.Shared.Next(3, 16))
				.ToList();

			foreach (var product in randomProducts)
			{
				campaign.Products.Add(product);
			}
		}

		context.SaleCampaigns.AddRange(campaigns);

		await context.SaveChangesAsync(default);
		await transaction.CommitAsync();
		return;
	}
}
