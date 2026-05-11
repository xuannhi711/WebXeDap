using FluentValidation.TestHelper;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Features.Catalog.Validators;
using WebXeDap.Application.Tests.Extensions;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Tests;

public sealed class CreateProductValidatorTests
{
	private readonly CreateProductValidator _validator;
	private readonly IApplicationDbContext _ctx;

	public CreateProductValidatorTests()
	{
		_ctx = TestApplicationDbContextFactory.CreateContext();
		_validator = new CreateProductValidator(_ctx);
	}

	[Fact]
	public async Task CreateProductValidator_Pass_WhenRequestIsValid()
	{
		var cat1 = new Category { Name = "Category 1" };
		var cat2 = new Category { Name = "Category 2" };
		await _ctx.AddCategoriesAsync([cat1, cat2]);
		var request = new CreateProductRequest(
			Name: "Valid Product",
			Description: "A valid product description",
			Price: 10,
			Quantity: 5,
			CategoryIDs: [cat1.ID, cat2.ID]
		);
		var result = await _validator.TestValidateAsync(request);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task CreateProductValidator_Fail_WhenRequestIsInvalid()
	{
		var request = new CreateProductRequest(
			Name: "",
			Description: null,
			Price: -1,
			Quantity: -1,
			CategoryIDs: [Random.Shared.Next(), Random.Shared.Next()]
		);
		var result = await _validator.TestValidateAsync(request);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(p => p.Name);
		result.ShouldHaveValidationErrorFor(p => p.Price);
		result.ShouldHaveValidationErrorFor(p => p.Quantity);
		result.ShouldHaveValidationErrorFor(p => p.CategoryIDs);
	}
}

public sealed class UpdateProductValidatorTests
{
	private readonly IApplicationDbContext _ctx;
	private readonly UpdateProductValidator _validator;

	public UpdateProductValidatorTests()
	{
		_ctx = TestApplicationDbContextFactory.CreateContext();
		_validator = new UpdateProductValidator(_ctx);
	}

	[Fact]
	public async Task UpdateProductValidator_Pass_WhenRequestIsValid()
	{
		var product = new Product
		{
			Name = "Existing Product",
			Price = 10,
			Quantity = 5,
		};
		await _ctx.AddProductAsync(product);
		var cat1 = new Category { Name = "Category 1" };
		var cat2 = new Category { Name = "Category 2" };
		await _ctx.AddCategoriesAsync([cat1, cat2]);

		var request = new UpdateProductRequest(
			ID: product.ID,
			Name: "Updated Product",
			Description: "An updated product description",
			Price: 20,
			Quantity: 10,
			CategoryIDs: [cat1.ID, cat2.ID],
			CurrencySymbol: "BTC"
		);
		var result = await _validator.TestValidateAsync(request);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task UpdateProductValidator_Fail_WhenRequestIsInvalid()
	{
		var request = new UpdateProductRequest(
			ID: Random.Shared.Next(),
			Name: "",
			Description: null,
			Price: -1,
			Quantity: -1,
			CategoryIDs: [Random.Shared.Next(), Random.Shared.Next()],
			CurrencySymbol: "BTC"
		);
		var result = await _validator.TestValidateAsync(request);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(p => p.ID);
		result.ShouldHaveValidationErrorFor(p => p.Name);
		result.ShouldHaveValidationErrorFor(p => p.Price);
		result.ShouldHaveValidationErrorFor(p => p.Quantity);
		result.ShouldHaveValidationErrorFor(p => p.CategoryIDs);
	}
}

public sealed class DeleteProductValidatorTests
{
	private readonly IApplicationDbContext _ctx;
	private readonly DeleteProductValidator _validator;

	public DeleteProductValidatorTests()
	{
		_ctx = TestApplicationDbContextFactory.CreateContext();
		_validator = new DeleteProductValidator(_ctx);
	}

	[Fact]
	public async Task DeleteProductValidator_Pass_WhenRequestIsValid()
	{
		var product = new Product
		{
			Name = "Existing Product",
			Price = 10,
			Quantity = 5,
		};
		await _ctx.AddProductAsync(product);

		var result = await _validator.TestValidateAsync(product.ID);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task DeleteProductValidator_Fail_WhenRequestIsInvalid()
	{
		var result = await _validator.TestValidateAsync(Random.Shared.Next());

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(r => r);
	}
}
