using Microsoft.AspNetCore.Mvc;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.WebAPI.Extensions;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Route("catalog/categories")]
public sealed class CategoriesController : ControllerBase
{
	private readonly ICategoryService _categories;

	public CategoriesController(ICategoryService categories)
	{
		_categories = categories;
	}

	[HttpGet]
	public async Task<ActionResult<List<CategoryResponse>>> List()
	{
		var categories = await _categories.ListAsync();
		return Ok(categories);
	}

	[HttpGet("hierarchy")]
	public async Task<ActionResult<List<HierarchyCategoryResponse>>> Hierarchy()
	{
		var categories = await _categories.ListHierarchyAsync();
		return Ok(categories);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<CategoryResponse>> GetById(int id)
	{
		var category = await _categories.GetByIDAsync(id);
		return this.ToActionResult(category);
	}

	[HttpPost]
	public async Task<ActionResult<CategoryResponse>> Create(
		[FromBody] CreateCategoryRequest request
	)
	{
		var result = await _categories.CreateAsync(
			new CreateCategoryCommand(request.Name, request.ParentCategoryID)
		);
		return this.ToActionResult(result);
	}

	[HttpPut("{id:int}")]
	public async Task<ActionResult<CategoryResponse>> Update(
		int id,
		[FromBody] UpdateCategoryRequest request
	)
	{
		var result = await _categories.UpdateAsync(
			new UpdateCategoryCommand(id, request.Name, request.ParentCategoryID)
		);
		return this.ToActionResult(result);
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		var result = await _categories.DeleteAsync(id);
		return this.ToActionResult(result);
	}

	public sealed record CreateCategoryRequest(string Name, int? ParentCategoryID);

	public sealed record UpdateCategoryRequest(string Name, int? ParentCategoryID);
}
