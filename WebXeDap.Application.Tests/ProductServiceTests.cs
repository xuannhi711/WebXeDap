using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.DTOs;
using WebXeDap.Application.Features.Catalog;
using WebXeDap.Application.Features.Catalog.Mapper;
using WebXeDap.Application.Features.Catalog.Queries;
using WebXeDap.Application.Tests.Extensions;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Tests;

public class ProductServiceCreateTests
{
	private readonly IApplicationDbContext _ctx;
	private readonly ProductService _service;
	private readonly ProductMapper _mapper;

	public ProductServiceCreateTests()
	{
		_ctx = TestApplicationDbContextFactory.CreateContext();
		_mapper = new ProductMapper(_ctx);
		_service = new ProductService(_ctx, _mapper);
	}

	[Fact]
	public async Task CreateAsync_Pass_WhenRequestIsValid()
	{
		var cat1 = new Category { Name = "Tires" };
		var cat2 = new Category { Name = "Bicycles" };
		await _ctx.AddCategoriesAsync([cat1, cat2]);
		var request = new CreateProductRequest(
			Name: "Tire A",
			Price: 100,
			Quantity: 10,
			CategoryIDs: [cat1.ID, cat2.ID],
			Description: null
		);

		var result = await _service.CreateAsync(request);
		Assert.NotNull(result);

		var newProduct = await _ctx.Products.ByID(result.ID).FirstOrDefaultAsync(default);
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
