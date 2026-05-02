using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Catalog;

namespace WebXeDap.Application.Tests;

public sealed class CatalogServiceTests
{
	[Fact]
	public async Task CreateProductAsync_CreatesProduct()
	{
		using var context = ServiceTestHelpers.CreateContext();
		var currentUser = new TestCurrentUserService { UserId = 1 };
		var service = new CatalogService(context, currentUser);

		var productId = await service.CreateProductAsync(
			name: "City Bike",
			description: "Commuter",
			price: 1200m,
			currencySymbol: null,
			quantity: 5,
			categoryIds: null,
			images: null,
			cancellationToken: CancellationToken.None
		);

		var product = await context.Products
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.ID == productId);

		Assert.NotNull(product);
		Assert.Equal("City Bike", product!.Name);
		Assert.Equal(5, product.Quantity);
		Assert.Equal("VNĐ", product.CurrencySymbol);
	}
}
