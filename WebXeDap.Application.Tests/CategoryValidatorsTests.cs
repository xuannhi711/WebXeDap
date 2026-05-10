using FluentValidation.TestHelper;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.DTOs;
using WebXeDap.Application.Features.Catalog.Validators;
using WebXeDap.Application.Tests.Extensions;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Tests;

public sealed class CreateCategoryValidatorTests
{
	private readonly CreateCategoryValidator _validator;
	private readonly IApplicationDbContext _ctx;

	public CreateCategoryValidatorTests()
	{
		_ctx = TestApplicationDbContextFactory.CreateContext();
		_validator = new CreateCategoryValidator(_ctx);
	}

	[Fact]
	public async Task CreateCategoryValidator_Fail_When_Name_Is_Empty()
	{
		var request = new CreateCategoryRequest(Name: "", ParentCategoryID: null);
		var result = await _validator.TestValidateAsync(request);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(c => c.Name);
	}

	[Fact]
	public async Task CreateCategoryValidator_Fail_When_ParentCategoryID_Does_Not_Exist()
	{
		var req = new CreateCategoryRequest(
			Name: "New Category",
			ParentCategoryID: Random.Shared.Next()
		);
		var result = await _validator.TestValidateAsync(req);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(c => c.ParentCategoryID);
	}

	[Fact]
	public async Task CreateCategoryValidator_Pass_When_Name_Is_Valid()
	{
		var req = new CreateCategoryRequest(Name: "New Category", ParentCategoryID: null);
		var result = await _validator.TestValidateAsync(req);

		result.ShouldNotHaveAnyValidationErrors();
		result.ShouldNotHaveValidationErrorFor(c => c.Name);
	}

	[Fact]
	public async Task CreateCategoryValidator_Pass_When_ParentCategoryID_Is_Valid()
	{
		var PARENT_CATEGORY = new Category { ID = Random.Shared.Next(), Name = "Parent Category" };
		await _ctx.AddCategoryAsync(PARENT_CATEGORY);

		var req = new CreateCategoryRequest(
			Name: "New Category",
			ParentCategoryID: PARENT_CATEGORY.ID
		);
		var result = await _validator.TestValidateAsync(req);

		result.ShouldNotHaveAnyValidationErrors();
		result.ShouldNotHaveValidationErrorFor(c => c.ParentCategoryID);
	}
}

public sealed class UpdateCategoryValidatorTests
{
	private readonly UpdateCategoryValidator _validator;
	private readonly IApplicationDbContext _ctx;

	public UpdateCategoryValidatorTests()
	{
		_ctx = TestApplicationDbContextFactory.CreateContext();
		_validator = new UpdateCategoryValidator(_ctx);
	}

	[Fact]
	public async Task UpdateCategoryValidator_Fail_When_ID_Does_Not_Exist()
	{
		var RandomCategory = new Category { ID = Random.Shared.Next(), Name = "Random Category" };
		await _ctx.AddCategoryAsync(RandomCategory);
		var req = new UpdateCategoryRequest(
			ID: Random.Shared.Next(),
			Name: "Updated Category",
			ParentCategoryID: null
		);
		var result = await _validator.TestValidateAsync(req);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(c => c.ID);
	}

	[Fact]
	public async Task UpdateCategoryValidator_Fail_When_Name_Is_Empty()
	{
		var EXISTING_CATEGORY = new Category
		{
			ID = Random.Shared.Next(),
			Name = "Existing Category",
		};
		await _ctx.AddCategoryAsync(EXISTING_CATEGORY);

		var req = new UpdateCategoryRequest(
			ID: EXISTING_CATEGORY.ID,
			Name: "",
			ParentCategoryID: null
		);

		var result = await _validator.TestValidateAsync(req);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(c => c.Name);
	}

	[Fact]
	public async Task UpdateCategoryValidator_Fail_When_ParentCategoryID_Is_Same_As_ID()
	{
		var EXISTING_CATEGORY = new Category
		{
			ID = Random.Shared.Next(),
			Name = "Existing Category",
		};
		await _ctx.AddCategoryAsync(EXISTING_CATEGORY);

		var req = new UpdateCategoryRequest(
			ID: EXISTING_CATEGORY.ID,
			Name: "Updated Category",
			ParentCategoryID: EXISTING_CATEGORY.ID
		);

		var result = await _validator.TestValidateAsync(req);
		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(c => c.ParentCategoryID);
	}

	[Fact]
	public async Task UpdateCategoryValidator_Fail_When_ParentCategoryID_Is_Invalid()
	{
		var EXISTING_CATEGORY = new Category
		{
			ID = Random.Shared.Next(),
			Name = "Existing Category",
		};
		await _ctx.AddCategoryAsync(EXISTING_CATEGORY);

		var req = new UpdateCategoryRequest(
			ID: EXISTING_CATEGORY.ID,
			Name: "Updated Category",
			ParentCategoryID: Random.Shared.Next()
		);
		var result = await _validator.TestValidateAsync(req);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(c => c.ParentCategoryID);
	}
}

public sealed class DeleteCategoryValidatorTests
{
	private readonly DeleteCategoryValidator _validator;
	private readonly IApplicationDbContext _ctx;

	public DeleteCategoryValidatorTests()
	{
		_ctx = TestApplicationDbContextFactory.CreateContext();
		_validator = new DeleteCategoryValidator(_ctx);
	}

	[Fact]
	public async Task DeleteCategoryValidator_Fail_When_ID_Does_Not_Exist()
	{
		const int NON_EXIST_CATEGORY_ID = 999;
		var result = await _validator.TestValidateAsync(NON_EXIST_CATEGORY_ID);

		result.ShouldHaveValidationErrors();
	}

	[Fact]
	public async Task DeleteCategoryValidator_Pass_When_ID_Exists()
	{
		var EXISTING_CATEGORY = new Category
		{
			ID = Random.Shared.Next(),
			Name = "Existing Category",
		};
		await _ctx.AddCategoryAsync(EXISTING_CATEGORY);

		var result = await _validator.TestValidateAsync(EXISTING_CATEGORY.ID);

		result.ShouldNotHaveAnyValidationErrors();
	}
}
