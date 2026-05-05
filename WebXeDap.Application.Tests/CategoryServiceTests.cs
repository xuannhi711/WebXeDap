using Moq;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Catalog;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Features.Catalog.Mapper;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Tests;

public class CategoryServiceTests
{
	private readonly Mock<ICategoryRepository> _repo;
	private readonly CategoryService _service;
	private readonly CategoryMapper _mapper;

	public CategoryServiceTests()
	{
		_repo = new Mock<ICategoryRepository>();
		_mapper = new CategoryMapper();
		_service = new CategoryService(_repo.Object, _mapper);
	}

	[Fact]
	public async Task CreateAsync_Pass_When_ValidRequestIsProvided()
	{
		var EXPECTED_CATEGORY = new Category { ID = 5, Name = "Electronics" };
		var request = new CreateCategoryRequest(Name: "Electronics", ParentCategoryID: null);

		_repo.SetupAddAnyAsyncToReturnACategory(EXPECTED_CATEGORY);

		var result = await _service.CreateAsync(request);

		Assert.Equal(EXPECTED_CATEGORY.ID, result.ID);
		Assert.Equal(EXPECTED_CATEGORY.Name, result.Name);
		Assert.Equal(EXPECTED_CATEGORY.ParentCategoryID, result.ParentCategoryID);
	}

	[Fact]
	public async Task CreateAsync_Fail_When_RepositoryThrowsException()
	{
		var request = new CreateCategoryRequest(Name: "Electronics", ParentCategoryID: null);

		_repo.SetupAddAnyAsyncToThrowException(
			new InvalidOperationException("An error occurred while creating the category.")
		);

		await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
	}

	[Fact]
	public async Task GetByIDAsync_Pass_When_CategoryExists()
	{
		var EXPECTED_CATEGORY = new Category { ID = 5, Name = "Electronics" };

		_repo.SetupGetByIdAsyncToReturnACategory(EXPECTED_CATEGORY.ID, EXPECTED_CATEGORY);

		var result = await _service.GetByIDAsync(EXPECTED_CATEGORY.ID);

		Assert.NotNull(result);
		Assert.Equal(EXPECTED_CATEGORY.ID, result.ID);
		Assert.Equal(EXPECTED_CATEGORY.Name, result.Name);
		Assert.Equal(EXPECTED_CATEGORY.ParentCategoryID, result.ParentCategoryID);
	}

	[Fact]
	public async Task GetByIDAsync_Fail_When_CategoryDoesNotExist()
	{
		const int NON_EXISTING_ID = 999;
		var result = await _service.GetByIDAsync(NON_EXISTING_ID);

		Assert.Null(result);
	}

	[Fact]
	public async Task ListAsync_Pass_With_3_Entries()
	{
		var categories = new List<Category>
		{
			new Category { ID = 1, Name = "Electronics" },
			new Category { ID = 2, Name = "Books" },
			new Category { ID = 3, Name = "Clothing" },
		};

		_repo.SetupListAsyncToReturnCategories(categories);

		var result = await _service.ListAsync();

		Assert.Equal(3, result.Count);
		Assert.Contains(result, c => c.ID == 1 && c.Name == "Electronics");
		Assert.Contains(result, c => c.ID == 2 && c.Name == "Books");
		Assert.Contains(result, c => c.ID == 3 && c.Name == "Clothing");
	}

	[Fact]
	public async Task ListAsync_Pass_With_EmptyList()
	{
		var result = await _service.ListAsync();
		Assert.Empty(result);
	}
}

static class CategoryServiceTestsHelper
{
	public static void SetupAddAnyAsyncToReturnACategory(
		this Mock<ICategoryRepository> mockRepo,
		Category category
	)
	{
		mockRepo
			.Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(category);
	}

	public static void SetupAddAnyAsyncToThrowException(
		this Mock<ICategoryRepository> mockRepo,
		Exception ex
	)
	{
		mockRepo
			.Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(ex);
	}

	public static void SetupGetByIdAsyncToReturnACategory(
		this Mock<ICategoryRepository> mockRepo,
		int id,
		Category? category
	)
	{
		mockRepo
			.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(category);
	}

	public static void SetupListAsyncToReturnCategories(
		this Mock<ICategoryRepository> mockRepo,
		List<Category> categories
	)
	{
		mockRepo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(categories);
	}
}
