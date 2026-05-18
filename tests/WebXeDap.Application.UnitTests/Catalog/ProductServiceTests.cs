using Microsoft.EntityFrameworkCore;
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
	private readonly IApplicationDbContext _ctx;
	private readonly IProductService _service;

	public ProductServiceCreateTests()
	{
		var fixture = new ApplicationTestFixture();
		_ctx = fixture.GetService<IApplicationDbContext>();
		_service = fixture.GetService<IProductService>();
	}

	[Fact]
	public async Task CreateAsync_Pass_WhenRequestIsValid()
	{
		var cat1 = new Category { Name = "Tires" };
		var cat2 = new Category { Name = "Bicycles" };
		await _ctx.AddCategoriesAsync([cat1, cat2]);
		var request = new CreateProductCommand(
			Name: "Tire A",
			Price: 100,
			Quantity: 10,
			CategoryIDs: [cat1.ID, cat2.ID],
			Description: null
		);

		var result = await _service.CreateAsync(request);
		Assert.True(result.TryPickValue(out var productResp));

		var newProduct = await _ctx.Products.ByID(productResp.ID).FirstOrDefaultAsync(default);
		Assert.NotNull(newProduct);
		Assert.Equal(request.Name, newProduct.Name);
		Assert.Equal(request.Price, newProduct.Price);
		Assert.Equal(request.Quantity, newProduct.Quantity);
		Assert.Equal(request.Description, newProduct.Description);
		Assert.Equal(request.CategoryIDs!.Length, newProduct.Categories.Count);
		Assert.Contains(newProduct.Categories, c => c.ID == cat1.ID && c.Name == cat1.Name);
		Assert.Contains(newProduct.Categories, c => c.ID == cat2.ID && c.Name == cat2.Name);
	}
}
