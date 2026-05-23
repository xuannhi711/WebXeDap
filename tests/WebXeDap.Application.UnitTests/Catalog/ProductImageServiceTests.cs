using Microsoft.EntityFrameworkCore;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.UnitTests.Extensions;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.UnitTests.Catalog;

[Trait("Category", "Unit")]
public sealed class ProductImageServiceTests
{
	private readonly IApplicationDbContext ctx;
	private readonly IProductService service;

	public ProductImageServiceTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<IProductService>();
	}

	[Fact]
	public async Task AddAndDeleteImage_Works()
	{
		var product = new Product
		{
			Name = "Test",
			Price = 1m,
			Quantity = 5,
		};
		await ctx.Products.AddAsync(product);
		await ctx.SaveChangesAsync(default);

		var addRes = await service.AddImageAsync(
			new CreateProductImageCommand(product.ID, "/uploads/test.jpg", 0)
		);
		Assert.True(addRes.IsOk);
		var img = addRes.Match(v => v, _ => null);
		Assert.NotNull(img);

		// ensure persisted
		var fromDb = await ctx.ProductImages.FirstOrDefaultAsync(i =>
			i.ProductID == product.ID && i.Key == "/uploads/test.jpg"
		);
		Assert.NotNull(fromDb);

		var delRes = await service.DeleteImageAsync(product.ID, fromDb.ID);
		Assert.True(delRes.IsOk);
		var key = delRes.Match(k => k, _ => null);
		Assert.Equal("/uploads/test.jpg", key);

		var check = await ctx.ProductImages.FindAsync(fromDb.ID);
		Assert.Null(check);
	}

	[Fact]
	public async Task AddImageAsync_Fails_WhenProductDoesNotExist()
	{
		var result = await service.AddImageAsync(
			new CreateProductImageCommand(999, "/uploads/missing.jpg", 0)
		);

		Assert.True(result.TryPickError(out var error));
		Assert.IsType<NotFoundError>(error);
	}

	[Fact]
	public async Task DeleteImageAsync_Fails_WhenImageDoesNotExist()
	{
		var product = new Product
		{
			Name = "Test",
			Price = 1m,
			Quantity = 5,
		};
		await ctx.Products.AddAsync(product);
		await ctx.SaveChangesAsync(default);

		var result = await service.DeleteImageAsync(product.ID, 12345);

		Assert.True(result.TryPickError(out var error));
		Assert.IsType<NotFoundError>(error);
	}

	[Fact]
	public async Task ListImagesAsync_ReturnsImagesOrderedByOrder()
	{
		var product = new Product
		{
			Name = "Test",
			Price = 1m,
			Quantity = 5,
		};
		await ctx.Products.AddAsync(product);
		await ctx.SaveChangesAsync(default);

		await ctx.ProductImages.AddRangeAsync(
			new ProductImage
			{
				ProductID = product.ID,
				Key = "/uploads/second.jpg",
				Order = 2,
			},
			new ProductImage
			{
				ProductID = product.ID,
				Key = "/uploads/first.jpg",
				Order = 1,
			}
		);
		await ctx.SaveChangesAsync(default);

		var images = await service.ListImagesAsync(product.ID);

		Assert.Collection(
			images,
			image => Assert.Equal("/uploads/first.jpg", image.URL),
			image => Assert.Equal("/uploads/second.jpg", image.URL)
		);
	}
}
