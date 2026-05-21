using Microsoft.EntityFrameworkCore;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Features.Catalog.Queries;
using WebXeDap.Application.UnitTests.Extensions;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.UnitTests.Catalog;

[Trait("Category", "Unit")]
public class ProductServiceCreateTests
{
	private readonly IApplicationDbContext ctx;
	private readonly IProductService service;

	public ProductServiceCreateTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<IProductService>();
	}

	[Fact]
	public async Task CreateAsync_Pass_WhenRequestIsValid()
	{
		var cat1 = await ctx.AddRandomCategoryAsync();
		var cat2 = await ctx.AddRandomCategoryAsync();
		var request = new CreateProductCommand(
			Name: "Tire A",
			Price: 100,
			Quantity: 10,
			CategoryIDs: [cat1.ID, cat2.ID],
			Description: null
		);

		var result = await service.CreateAsync(request);
		Assert.True(result.TryPickValue(out var productResp));

		var newProduct = await ctx.Products.FindAsync(productResp.ID);
		Assert.NotNull(newProduct);
		Assert.Equal(request.Name, newProduct.Name);
		Assert.Equal(request.Price, newProduct.Price);
		Assert.Equal(request.Quantity, newProduct.Quantity);
		Assert.Equal(request.Description, newProduct.Description);
		Assert.Equal(request.CategoryIDs!.Count, newProduct.Categories!.Count);
		Assert.Contains(newProduct.Categories, c => c.ID == cat1.ID && c.Name == cat1.Name);
		Assert.Contains(newProduct.Categories, c => c.ID == cat2.ID && c.Name == cat2.Name);
	}

	[Fact]
	public async Task CreateAsync_Fail_WhenRequestIsInvalid()
	{
		var request = new CreateProductCommand(
			Name: "",
			Price: -1,
			Quantity: -5,
			CategoryIDs: [Random.Shared.Next()],
			Description: null
		);

		var result = await service.CreateAsync(request);
		Assert.True(result.TryPickError(out var error));
		var validationError = Assert.IsType<ValidationError>(error);
		Assert.True(validationError.Errors.ContainsKey(nameof(CreateProductCommand.Name)));
		Assert.True(validationError.Errors.ContainsKey(nameof(CreateProductCommand.Price)));
		Assert.True(validationError.Errors.ContainsKey(nameof(CreateProductCommand.Quantity)));
		Assert.True(validationError.Errors.ContainsKey(nameof(CreateProductCommand.CategoryIDs)));
	}
}

[Trait("Category", "Unit")]
public class ProductServiceReadTests
{
	private readonly IApplicationDbContext ctx;
	private readonly IProductService service;

	public ProductServiceReadTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<IProductService>();
	}

	[Fact]
	public async Task GetByIDAsync_Pass_WhenProductExists()
	{
		var product = new Product
		{
			Name = "Road Bike",
			Price = 200,
			Quantity = 5,
		};
		await ctx.AddProductAsync(product);

		var result = await service.GetByIDAsync(product.ID);
		Assert.True(result.TryPickValue(out var found));
		Assert.Equal(product.ID, found.ID);
		Assert.Equal(product.Name, found.Name);
	}

	[Fact]
	public async Task GetByIDAsync_Fail_WhenProductDoesNotExist()
	{
		var result = await service.GetByIDAsync(Random.Shared.Next());
		Assert.True(result.TryPickError(out var error));
		Assert.IsType<NotFoundError>(error);
	}
}

[Trait("Category", "Unit")]
public class ProductServiceFilterTests
{
	private readonly IApplicationDbContext ctx;
	private readonly IProductService service;

	public ProductServiceFilterTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<IProductService>();
	}

	[Fact]
	public async Task FilterAsync_ReturnsMatches_ByKeyword()
	{
		var bike = new Product
		{
			Name = "Bike Pro",
			Price = 120,
			Quantity = 3,
		};
		var helmet = new Product
		{
			Name = "Helmet",
			Price = 30,
			Quantity = 10,
		};
		await ctx.AddProductAsync(bike);
		await ctx.AddProductAsync(helmet);

		var result = await service.FilterAsync(
			new FilterProductCommand(Keyword: "Bike", CategoryIDs: null, MinPrice: null, MaxPrice: null),
			page: 1,
			size: 10
		);

		Assert.Single(result);
		Assert.Equal(bike.ID, result[0].ID);
	}

	[Fact]
	public async Task FilterAsync_ReturnsMatches_ByCategory()
	{
		var cat1 = await ctx.AddRandomCategoryAsync();
		var cat2 = await ctx.AddRandomCategoryAsync();
		var roadBike = new Product
		{
			Name = "Road Bike",
			Price = 250,
			Quantity = 2,
			Categories = [cat1],
		};
		var mountainBike = new Product
		{
			Name = "Mountain Bike",
			Price = 300,
			Quantity = 1,
			Categories = [cat2],
		};
		await ctx.AddProductAsync(roadBike);
		await ctx.AddProductAsync(mountainBike);

		var result = await service.FilterAsync(
			new FilterProductCommand(
				Keyword: null,
				CategoryIDs: [cat1.ID],
				MinPrice: null,
				MaxPrice: null
			),
			page: 1,
			size: 10
		);

		Assert.Single(result);
		Assert.Equal(roadBike.ID, result[0].ID);
	}

	[Fact]
	public async Task FilterAsync_SortsByPriceDescending()
	{
		var low = new Product
		{
			Name = "Low",
			Price = 50,
			Quantity = 1,
		};
		var high = new Product
		{
			Name = "High",
			Price = 150,
			Quantity = 1,
		};
		await ctx.AddProductAsync(low);
		await ctx.AddProductAsync(high);

		var result = await service.FilterAsync(
			new FilterProductCommand(
				Keyword: null,
				CategoryIDs: null,
				MinPrice: null,
				MaxPrice: null,
				SortBy: "price",
				IsAscending: false
			),
			page: 1,
			size: 10
		);

		Assert.Equal(high.ID, result[0].ID);
		Assert.Equal(low.ID, result[1].ID);
	}

	[Fact]
	public async Task CountAsync_ReturnsCount_WithMinPrice()
	{
		var low = new Product
		{
			Name = "Low",
			Price = 10,
			Quantity = 1,
		};
		var high = new Product
		{
			Name = "High",
			Price = 200,
			Quantity = 1,
		};
		await ctx.AddProductAsync(low);
		await ctx.AddProductAsync(high);

		var count = await service.CountAsync(
			new FilterProductCommand(
				Keyword: null,
				CategoryIDs: null,
				MinPrice: 100,
				MaxPrice: null
			)
		);

		Assert.Equal(1, count);
	}
}

[Trait("Category", "Unit")]
public class ProductServiceUpdateTests
{
	private readonly IApplicationDbContext ctx;
	private readonly IProductService service;

	public ProductServiceUpdateTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<IProductService>();
	}

	[Fact]
	public async Task UpdateAsync_Pass_WhenRequestIsValid()
	{
		var originalCategory = await ctx.AddRandomCategoryAsync();
		var newCategory = await ctx.AddRandomCategoryAsync();
		var product = new Product
		{
			Name = "Old Name",
			Description = "Old Desc",
			Price = 100,
			Quantity = 2,
			Categories = [originalCategory],
		};
		await ctx.AddProductAsync(product);

		var request = new UpdateProductCommand(
			ID: product.ID,
			Name: "New Name",
			Description: "New Desc",
			Price: 250,
			Quantity: 5,
			CategoryIDs: [newCategory.ID],
			CurrencySymbol: "$"
		);

		var result = await service.UpdateAsync(request);
		Assert.True(result.TryPickValue(out var updated));
		Assert.Equal(request.ID, updated.ID);
		Assert.Equal(request.Name, updated.Name);
		Assert.Equal(request.Description, updated.Description);
		Assert.Equal(request.Price, updated.Price);
		Assert.Equal(request.Quantity, updated.Quantity);
		Assert.Equal(request.CurrencySymbol, updated.CurrencySymbol);

		var stored = await ctx.Products.Include(p => p.Categories).FirstAsync(
			p => p.ID == product.ID
		);
		Assert.Equal(request.Name, stored.Name);
		Assert.Equal(request.Price, stored.Price);
		Assert.Equal(request.Quantity, stored.Quantity);
		Assert.Single(stored.Categories!);
		Assert.Equal(newCategory.ID, stored.Categories!.First().ID);
	}

	[Fact]
	public async Task UpdateAsync_Fail_WhenIDIsInvalid()
	{
		var request = new UpdateProductCommand(
			ID: Random.Shared.Next(),
			Name: "Updated",
			Description: null,
			Price: 10,
			Quantity: 1,
			CategoryIDs: null,
			CurrencySymbol: null
		);

		var result = await service.UpdateAsync(request);
		Assert.True(result.TryPickError(out var error));
		var validationError = Assert.IsType<ValidationError>(error);
		Assert.True(validationError.Errors.ContainsKey(nameof(UpdateProductCommand.ID)));
	}
}

[Trait("Category", "Unit")]
public class ProductServiceDeleteTests
{
	private readonly IApplicationDbContext ctx;
	private readonly IProductService service;

	public ProductServiceDeleteTests()
	{
		var fixture = new ApplicationTestFixture();
		ctx = fixture.GetService<IApplicationDbContext>();
		service = fixture.GetService<IProductService>();
	}

	[Fact]
	public async Task DeleteAsync_Pass_WhenProductExists()
	{
		var product = new Product
		{
			Name = "Disposable",
			Price = 20,
			Quantity = 1,
		};
		await ctx.AddProductAsync(product);

		var result = await service.DeleteAsync(product.ID);
		Assert.True(result.IsOk);
		var deleted = await ctx.Products.FindAsync(product.ID);
		Assert.Null(deleted);
	}

	[Fact]
	public async Task DeleteAsync_Fail_WhenProductDoesNotExist()
	{
		var result = await service.DeleteAsync(Random.Shared.Next());
		Assert.True(result.TryPickError(out var error));
		Assert.IsType<ValidationError>(error);
	}
}
