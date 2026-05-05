using Moq;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Features.Catalog;
using WebXeDap.Application.DTOs;
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
		_repo.SetupListAsyncToReturnCategories([]);

		var result = await _service.ListAsync();
		Assert.Empty(result);
	}

	[Fact]
	public async Task ListHierarchyAsync_Pass()
	{
		var categories = new List<Category>
		{
			new Category { ID = 1, Name = "A" },
			new Category
			{
				ID = 2,
				Name = "A1",
				ParentCategoryID = 1,
			},
			new Category
			{
				ID = 3,
				Name = "A2",
				ParentCategoryID = 1,
			},
			new Category
			{
				ID = 4,
				Name = "A1.1",
				ParentCategoryID = 2,
			},
			new Category { ID = 5, Name = "B" },
			new Category { ID = 6, Name = "C" },
		};
		_repo.SetupListAsyncToReturnCategories(categories);

		var resp = await _service.ListHierarchyAsync();

		// root
		Assert.Equal(3, resp.Count);
		Assert.Collection(
			resp,
			i =>
			{
				Assert.Equal("A", i.Name);
				Assert.Equal(2, i.Children.Count);
				Assert.Equal("A1", i.Children[0].Name);
				Assert.Equal("A2", i.Children[1].Name);
				Assert.Single(i.Children[0].Children);
				Assert.Equal("A1.1", i.Children[0].Children[0].Name);
			},
			i => Assert.Equal("B", i.Name),
			i => Assert.Equal("C", i.Name)
		);
	}

	[Fact]
	public async Task UpdateAsync_Pass_When_RequestIsValid()
	{
		var request = new UpdateCategoryRequest(ID: 7, Name: "Tires", ParentCategoryID: 2);
		_repo.SetupUpdateAsyncToReturnRowsAffected(1);

		var result = await _service.UpdateAsync(request);

		Assert.Equal(1, result);
		_repo.Verify(
			r =>
				r.UpdateAsync(
					It.Is<Category>(c =>
						c.ID == request.ID
						&& c.Name == request.Name
						&& c.ParentCategoryID == request.ParentCategoryID
					),
					It.IsAny<CancellationToken>()
				),
			Times.Once
		);
	}

	[Fact]
	public async Task UpdateAsync_Fail_When_RepositoryThrowsException()
	{
		var request = new UpdateCategoryRequest(ID: 7, Name: "Tires", ParentCategoryID: 2);
		_repo.SetupUpdateAsyncToThrowException(new InvalidOperationException("Update failed."));

		await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(request));
	}

	[Fact]
	public async Task DeleteAsync_Pass_When_CategoryExists()
	{
		var existing = new Category { ID = 3, Name = "Accessories" };
		_repo.SetupGetByIdAsyncToReturnACategory(existing.ID, existing);
		_repo.SetupDeleteAsyncToReturnRowsAffected(1);

		var result = await _service.DeleteAsync(existing.ID);

		Assert.True(result);
		_repo.Verify(r => r.DeleteAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_Fail_When_CategoryDoesNotExist()
	{
		const int MISSING_ID = 404;

		var result = await _service.DeleteAsync(MISSING_ID);

		Assert.False(result);
		_repo.Verify(
			r => r.DeleteAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
			Times.Never
		);
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

	public static void SetupUpdateAsyncToReturnRowsAffected(
		this Mock<ICategoryRepository> mockRepo,
		int rowsAffected
	)
	{
		mockRepo
			.Setup(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(rowsAffected);
	}

	public static void SetupUpdateAsyncToThrowException(
		this Mock<ICategoryRepository> mockRepo,
		Exception ex
	)
	{
		mockRepo
			.Setup(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(ex);
	}

	public static void SetupDeleteAsyncToReturnRowsAffected(
		this Mock<ICategoryRepository> mockRepo,
		int rowsAffected
	)
	{
		mockRepo
			.Setup(r => r.DeleteAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(rowsAffected);
	}
}
