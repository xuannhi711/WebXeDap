// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

// Note: Khi mock repo, cần phải lấy id của spec ra vì
// moq so sánh theo ref, nếu không sẽ bị fail dù cùng id

// e.g.
//     const int PARENT_CATEGORY_ID = 6789;
//     var spec = new CategoryByIDSpec(PARENT_CATEGORY_ID);
//     _mockRepo
//         .Setup(r => r.AnyAsync(
//             It.Is<CategoryByIDSpec>(s => s.ID == PARENT_CATEGORY_ID),
//             default))
//         .ReturnsAsync(true);

// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

using Moq;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Features.Catalog.Specs;
using WebXeDap.Application.Features.Catalog.Validators;
using FluentValidation.TestHelper;


namespace WebXeDap.Application.Tests;

public sealed class CategoryValidatorsTests
{
	private readonly Mock<ICategoryRepository> _mockRepo;
	private readonly CreateCategoryValidator _validator;

	public CategoryValidatorsTests()
	{
		_mockRepo = new Mock<ICategoryRepository>();
		_validator = new CreateCategoryValidator(_mockRepo.Object);
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
		var req = new CreateCategoryRequest(Name: "New Category", ParentCategoryID: 999);
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
		const int PARENT_CATEGORY_ID = 6789;
		_mockRepo.SetupCategoryExists(PARENT_CATEGORY_ID);

		var req = new CreateCategoryRequest(
			Name: "New Category",
			ParentCategoryID: PARENT_CATEGORY_ID
		);
		var result = await _validator.TestValidateAsync(req);

		result.ShouldNotHaveAnyValidationErrors();
		result.ShouldNotHaveValidationErrorFor(c => c.ParentCategoryID);
	}
}

public sealed class UpdateCategoryValidatorTests
{
	private readonly Mock<ICategoryRepository> _mockRepo;
	private readonly UpdateCategoryValidator _validator;

	public UpdateCategoryValidatorTests()
	{
		_mockRepo = new Mock<ICategoryRepository>();
		_validator = new UpdateCategoryValidator(_mockRepo.Object);
	}

	[Fact]
	public async Task UpdateCategoryValidator_Fail_When_ID_Does_Not_Exist()
	{
		var req = new UpdateCategoryRequest(
			ID: 999,
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
		const int EXISTING_CATEGORY_ID = 1;
		_mockRepo.SetupCategoryExists(EXISTING_CATEGORY_ID);

		var req = new UpdateCategoryRequest(
			ID: EXISTING_CATEGORY_ID,
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
		const int EXISTING_CATEGORY_ID = 1;
		_mockRepo.SetupCategoryExists(EXISTING_CATEGORY_ID);

		var req = new UpdateCategoryRequest(
			ID: EXISTING_CATEGORY_ID,
			Name: "Updated Category",
			ParentCategoryID: EXISTING_CATEGORY_ID
		);

		var result = await _validator.TestValidateAsync(req);
		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(c => c.ParentCategoryID);
	}

	[Fact]
	public async Task UpdateCategoryValidator_Fail_When_ParentCategoryID_Is_Invalid()
	{
		const int EXISTING_CATEGORY_ID = 1;
		_mockRepo.SetupCategoryExists(EXISTING_CATEGORY_ID);

		var req = new UpdateCategoryRequest(ID: 1, Name: "Updated Category", ParentCategoryID: 999);
		var result = await _validator.TestValidateAsync(req);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(c => c.ParentCategoryID);
	}
}

public sealed class DeleteCategoryValidatorTests
{
	private readonly Mock<ICategoryRepository> _mockRepo;
	private readonly DeleteCategoryValidator _validator;

	public DeleteCategoryValidatorTests()
	{
		_mockRepo = new Mock<ICategoryRepository>();
		_validator = new DeleteCategoryValidator(_mockRepo.Object);
	}

	[Fact]
	public async Task DeleteCategoryValidator_Fail_When_ID_Does_Not_Exist()
	{
		var req = new DeleteCategoryRequest(ID: 999);
		var result = await _validator.TestValidateAsync(req);

		result.ShouldHaveValidationErrors();
		result.ShouldHaveValidationErrorFor(c => c.ID);
	}

	[Fact]
	public async Task DeleteCategoryValidator_Pass_When_ID_Exists()
	{
		const int EXISTING_CATEGORY_ID = 1;
		_mockRepo.SetupCategoryExists(EXISTING_CATEGORY_ID);

		var req = new DeleteCategoryRequest(ID: EXISTING_CATEGORY_ID);
		var result = await _validator.TestValidateAsync(req);

		result.ShouldNotHaveAnyValidationErrors();
	}
}

static class CategoryValidatorsTestsHelper
{
	public static void SetupCategoryExists(this Mock<ICategoryRepository> mock, int categoryID)
	{
		mock.Setup(r => r.AnyAsync(It.Is<CategoryByIDSpec>(s => s.ID == categoryID), default))
			.ReturnsAsync(true);
	}
}
