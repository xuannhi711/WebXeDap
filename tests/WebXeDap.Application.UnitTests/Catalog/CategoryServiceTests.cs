using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.UnitTests.Extensions;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.UnitTests.Catalog;

[Trait("Category", "Unit")]
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
		Assert.True(result.TryPickValue(out var created));
		Assert.NotEqual(0, created.ID);
		Assert.Equal(request.Name, created.Name);
		Assert.Equal(parentCategory.ID, created.ParentCategoryID);
	}

	[Fact]
	public async Task CreateAsync_Fail_WhenRequestIsInvalid()
	{
		var cmd = new CreateCategoryCommand(Name: "", ParentCategoryID: Random.Shared.Next());
		var result = await _service.CreateAsync(cmd);
		Assert.True(result.TryPickError(out var error));
		var validationError = Assert.IsType<ValidationError>(error);
		Assert.True(validationError.Errors.ContainsKey(nameof(CreateCategoryCommand.Name)));
		Assert.True(
			validationError.Errors.ContainsKey(nameof(CreateCategoryCommand.ParentCategoryID))
		);
	}
}

[Trait("Category", "Unit")]
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
		Assert.True(result.TryPickValue(out var found));
		Assert.Equal(category.ID, found.ID);
		Assert.Equal(category.Name, found.Name);
		Assert.Equal(category.ParentCategoryID, found.ParentCategoryID);
	}

	[Fact]
	public async Task GetByIDAsync_Fail_WhenCategoryDoesNotExist()
	{
		var result = await _service.GetByIDAsync(Random.Shared.Next());
		Assert.True(result.TryPickError(out var error));
		Assert.IsType<NotFoundError>(error);
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
		var rootResp = resp.Find(c => c.ID == root.ID);
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

[Trait("Category", "Unit")]
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

		var req = new UpdateCategoryCommand(Name: "Updated Category", ParentCategoryID: null);
		var result = await _service.UpdateAsync(EXISTING_CATEGORY.ID, req);
		Assert.True(result.TryPickValue(out var updated));
		Assert.Equal(EXISTING_CATEGORY.ID, updated.ID);
		Assert.Equal(req.Name, updated.Name);
		Assert.Equal(req.ParentCategoryID, updated.ParentCategoryID);
	}

	[Fact]
	public async Task UpdateAsync_Fail_WhenIDIsInvalid()
	{
		var req = new UpdateCategoryCommand(Name: "Updated Category", ParentCategoryID: null);
		var result = await _service.UpdateAsync(Random.Shared.Next(), req);
		Assert.True(result.TryPickError(out var error));
		Assert.IsType<NotFoundError>(error);
	}
}

[Trait("Category", "Unit")]
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
		Assert.True(result.IsOk);
		var deletedCategory = await _ctx.Categories.FindAsync(EXISTING_CATEGORY.ID);
		Assert.Null(deletedCategory);
	}

	[Fact]
	public async Task DeleteAsync_Fail_When_CategoryDoesNotExist()
	{
		var result = await _service.DeleteAsync(Random.Shared.Next());
		Assert.True(result.TryPickError(out var error));
		Assert.IsType<NotFoundError>(error);
	}
}
