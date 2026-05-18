using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Tests.Extensions;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Tests.Catalog;

public class CategoryServiceCreateTests
{
	private readonly IApplicationDbContext _ctx;
	private readonly ICategoryService _service;

	public CategoryServiceCreateTests()
	{
		var fixture = new ApplicationTestFixture();
		_ctx = fixture.GetService<IApplicationDbContext>();
		_service = fixture.GetService<ICategoryService>();
	}

	[Fact]
	public async Task CreateAsync_Pass_WhenRequestIsValid()
	{
		var parentCategory = new Category { Name = "Vehicles" };
		await _ctx.AddCategoryAsync(parentCategory);
		var request = new CreateCategoryCommand(Name: "Tires", ParentCategoryID: parentCategory.ID);

		var result = await _service.CreateAsync(request);

		Assert.NotNull(result);
		Assert.NotEqual(0, result.ID);
		Assert.Equal(request.Name, result.Name);
		Assert.Equal(parentCategory.ID, result.ParentCategoryID);
	}
}

public class CategoryServiceReadTests
{
	private readonly IApplicationDbContext _ctx;
	private readonly ICategoryService _service;

	public CategoryServiceReadTests()
	{
		var fixture = new ApplicationTestFixture();
		_ctx = fixture.GetService<IApplicationDbContext>();
		_service = fixture.GetService<ICategoryService>();
	}

	[Fact]
	public async Task GetByIDAsync_Pass_WhenCategoryExists()
	{
		var category = new Category { Name = "Tires" };
		await _ctx.AddCategoryAsync(category);

		var result = await _service.GetByIDAsync(category.ID);

		Assert.NotNull(result);
		Assert.Equal(category.ID, result.ID);
		Assert.Equal(category.Name, result.Name);
		Assert.Equal(category.ParentCategoryID, result.ParentCategoryID);
	}

	[Fact]
	public async Task GetByIDAsync_Fail_WhenCategoryDoesNotExist()
	{
		var result = await _service.GetByIDAsync(Random.Shared.Next());

		Assert.Null(result);
	}

	[Fact]
	public async Task ListAsync_Pass_With3Entries()
	{
		var categories = new List<Category>
		{
			new() { Name = "Electronics" },
			new() { Name = "Books" },
			new() { Name = "Clothing" },
		};
		await _ctx.AddCategoriesAsync(categories);

		var result = await _service.ListAsync();

		Assert.Contains(result, c => c.ID == categories[0].ID && c.Name == "Electronics");
		Assert.Contains(result, c => c.ID == categories[1].ID && c.Name == "Books");
		Assert.Contains(result, c => c.ID == categories[2].ID && c.Name == "Clothing");
	}

	[Fact]
	public async Task ListHierarchyAsync_Pass()
	{
		var root = new Category { Name = "A" };
		await _ctx.AddCategoryAsync(root);
		var child1 = new Category { Name = "A1", ParentCategoryID = root.ID };
		var child2 = new Category { Name = "A2", ParentCategoryID = root.ID };
		await _ctx.AddCategoriesAsync([child1, child2]);
		var grandchild1 = new Category { Name = "A1.1", ParentCategoryID = child1.ID };
		await _ctx.AddCategoryAsync(grandchild1);

		var resp = await _service.ListHierarchyAsync();
		var rootResp = resp.FirstOrDefault(c => c.ID == root.ID);
		Assert.NotNull(rootResp);

		Assert.Collection(
			rootResp.Children,
			i =>
			{
				Assert.Equal("A1", i.Name);
				Assert.Single(i.Children);
				Assert.Equal("A1.1", i.Children[0].Name);
			},
			i =>
			{
				Assert.Equal("A2", i.Name);
				Assert.Empty(i.Children);
			}
		);
	}
}

public class CategoryServiceUpdateTests
{
	private readonly IApplicationDbContext _ctx;
	private readonly ICategoryService _service;

	public CategoryServiceUpdateTests()
	{
		var fixture = new ApplicationTestFixture();
		_ctx = fixture.GetService<IApplicationDbContext>();
		_service = fixture.GetService<ICategoryService>();
	}

	[Fact]
	public async Task UpdateAsync_Pass_WhenRequestIsValid()
	{
		var EXISTING_CATEGORY = new Category { Name = "Existing Category" };
		await _ctx.AddCategoryAsync(EXISTING_CATEGORY);

		var req = new UpdateCategoryCommand(
			ID: EXISTING_CATEGORY.ID,
			Name: "Updated Category",
			ParentCategoryID: null
		);
		var result = await _service.UpdateAsync(req);

		Assert.NotNull(result);
		Assert.Equal(req.ID, result.ID);
		Assert.Equal(req.Name, result.Name);
		Assert.Equal(req.ParentCategoryID, result.ParentCategoryID);
	}

	[Fact]
	public async Task UpdateAsync_Fail_WhenIDIsInvalid()
	{
		var req = new UpdateCategoryCommand(
			ID: Random.Shared.Next(),
			Name: "Updated Category",
			ParentCategoryID: null
		);
		var result = await _service.UpdateAsync(req);
		Assert.Null(result);
	}
}

public class CategoryServiceDeleteTests
{
	private readonly IApplicationDbContext _ctx;
	private readonly ICategoryService _service;

	public CategoryServiceDeleteTests()
	{
		var fixture = new ApplicationTestFixture();
		_ctx = fixture.GetService<IApplicationDbContext>();
		_service = fixture.GetService<ICategoryService>();
	}

	[Fact]
	public async Task DeleteAsync_Pass_When_CategoryExists()
	{
		var EXISTING_CATEGORY = new Category { Name = "Existing Category" };
		await _ctx.AddCategoryAsync(EXISTING_CATEGORY);

		var result = await _service.DeleteAsync(EXISTING_CATEGORY.ID);

		Assert.True(result);
		var deletedCategory = await _ctx.Categories.FindAsync(EXISTING_CATEGORY.ID);
		Assert.Null(deletedCategory);
	}

	[Fact]
	public async Task DeleteAsync_Fail_When_CategoryDoesNotExist()
	{
		var result = await _service.DeleteAsync(Random.Shared.Next());
		Assert.False(result);
	}
}
