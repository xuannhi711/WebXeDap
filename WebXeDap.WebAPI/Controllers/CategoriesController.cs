using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Domain.Constants;
using WebXeDap.WebAPI.Extensions;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
	private readonly ICategoryService categoriesService;

	public CategoriesController(ICategoryService categoriesService)
	{
		this.categoriesService = categoriesService;
	}

	[HttpGet]
	public async Task<ActionResult<List<CategoryResponse>>> List()
	{
		var categories = await categoriesService.ListAsync();
		return Ok(categories);
	}

	[HttpGet("hierarchy")]
	public async Task<ActionResult<List<HierarchyCategoryResponse>>> Hierarchy()
	{
		var categories = await categoriesService.ListHierarchyAsync();
		return Ok(categories);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<CategoryResponse>> GetById(int id)
	{
		var res = await categoriesService.GetByIDAsync(id);
		return res.Match(Ok, this.MatchErrorResult);
	}

	[HttpPost]
	[Authorize(Roles = ROLES.ADMIN)]
	public async Task<ActionResult<CategoryResponse>> Create(
		[FromBody] CreateCategoryCommand request
	)
	{
		var res = await categoriesService.CreateAsync(request);
		return res.Match(val => Created(nameof(GetById), val), this.MatchErrorResult);
	}

	[HttpPut("{id:int}")]
	[Authorize(Roles = ROLES.ADMIN)]
	public async Task<ActionResult<CategoryResponse>> Update(
		int id,
		[FromBody] UpdateCategoryCommand request
	)
	{
		var result = await categoriesService.UpdateAsync(id, request);
		return result.Match(Ok, this.MatchErrorResult);
	}

	[HttpDelete("{id:int}")]
	[Authorize(Roles = ROLES.ADMIN)]
	public async Task<IActionResult> Delete(int id)
	{
		var result = await categoriesService.DeleteAsync(id);
		return result.Match(_ => NoContent(), this.MatchErrorResult);
	}
}
