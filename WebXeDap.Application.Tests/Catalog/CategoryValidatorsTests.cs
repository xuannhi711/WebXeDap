using FluentValidation.TestHelper;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Features.Catalog.Validators;
using WebXeDap.Application.Tests.Extensions;
using WebXeDap.Application.Tests.Fixtures;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Tests.Catalog;

public sealed class CreateCategoryValidatorTests
{
	private readonly CreateCategoryValidator _validator;
	private readonly IApplicationDbContext _ctx;

	public CreateCategoryValidatorTests()
	{
		var fixture = new ApplicationTestFixture();
		_ctx = fixture.GetService<IApplicationDbContext>();
		_validator = fixture.GetService<CreateCategoryValidator>();
	}

	[Fact]
	public async Task CreateCategoryValidator_Pass_WhenRequestIsValid()
	{
		var parentCategory = new Category { Name = "Parent Category" };
		await _ctx.AddCategoryAsync(parentCategory);
		var req = new CreateCategoryCommand(
			Name: "New Category",
			ParentCategoryID: parentCategory.ID
		);

		var result = await _validator.TestValidateAsync(req);
		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task CreateCategoryValidator_Fail_WhenRequestIsInvalid()
	{
		var req = new CreateCategoryCommand(Name: "", ParentCategoryID: Random.Shared.Next());
		var result = await _validator.TestValidateAsync(req);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(c => c.Name);
		result.ShouldHaveValidationErrorFor(c => c.ParentCategoryID);
	}
}

public sealed class UpdateCategoryValidatorTests
{
	private readonly UpdateCategoryValidator _validator;
	private readonly IApplicationDbContext _ctx;

	public UpdateCategoryValidatorTests()
	{
		var fixture = new ApplicationTestFixture();
		_ctx = fixture.GetService<IApplicationDbContext>();
		_validator = fixture.GetService<UpdateCategoryValidator>();
	}

	[Fact]
	public async Task UpdateCategoryValidator_Pass_WhenRequestIsValid()
	{
		var parentCategory = new Category { Name = "Parent Category" };
		var toBeParentCategory = new Category { Name = "To Be Parent Category" };
		await _ctx.AddCategoriesAsync([parentCategory, toBeParentCategory]);
		var toUpdateCategory = new Category
		{
			Name = "Existing Category",
			ParentCategoryID = parentCategory.ID,
		};
		await _ctx.AddCategoryAsync(toUpdateCategory);

		Assert.NotEqual(toUpdateCategory.ID, toBeParentCategory.ID);

		var req = new UpdateCategoryCommand(
			ID: toUpdateCategory.ID,
			Name: "Updated Category",
			ParentCategoryID: toBeParentCategory.ID
		);

		var result = await _validator.TestValidateAsync(req);
		result.ShouldNotHaveAnyValidationErrors();

		req = new UpdateCategoryCommand(
			ID: toUpdateCategory.ID,
			Name: "Updated Category",
			ParentCategoryID: null
		);
		result = await _validator.TestValidateAsync(req);
		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public async Task UpdateCategoryValidator_Fail_WhenRequestIsInvalid()
	{
		var req = new UpdateCategoryCommand(
			ID: Random.Shared.Next(),
			Name: "",
			ParentCategoryID: Random.Shared.Next()
		);

		var result = await _validator.TestValidateAsync(req);
		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(c => c.ID);
		result.ShouldHaveValidationErrorFor(c => c.Name);
		result.ShouldHaveValidationErrorFor(c => c.ParentCategoryID);

		var duplicateID = Random.Shared.Next();
		req = new UpdateCategoryCommand(
			ID: duplicateID,
			Name: "Valid Name",
			ParentCategoryID: duplicateID
		);
		result = await _validator.TestValidateAsync(req);
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
		var fixture = new ApplicationTestFixture();
		_ctx = fixture.GetService<IApplicationDbContext>();
		_validator = fixture.GetService<DeleteCategoryValidator>();
	}

	[Fact]
	public async Task DeleteCategoryValidator_Fail_When_ID_Does_Not_Exist()
	{
		var nonExistCategoryID = Random.Shared.Next();
		var result = await _validator.TestValidateAsync(nonExistCategoryID);

		result.ShouldHaveValidationErrors();
	}

	[Fact]
	public async Task DeleteCategoryValidator_Pass_When_ID_Exists()
	{
		var existingCategory = new Category
		{
			ID = Random.Shared.Next(),
			Name = "Existing Category",
		};
		await _ctx.AddCategoryAsync(existingCategory);

		var result = await _validator.TestValidateAsync(existingCategory.ID);

		result.ShouldNotHaveAnyValidationErrors();
	}
}
