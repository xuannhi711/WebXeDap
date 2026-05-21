using Microsoft.EntityFrameworkCore;
using Util.Primitives.ResultType;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Extensions;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Features.Catalog.Mapper;
using WebXeDap.Application.Features.Catalog.Queries;
using WebXeDap.Application.Features.Catalog.Validators;

namespace WebXeDap.Application.Features.Catalog;

public class CategoryService : ICategoryService
{
	private readonly IApplicationDbContext _ctx;
	private readonly CategoryMapper _mapper;
	private readonly CreateCategoryValidator _createValidator;
	private readonly UpdateCategoryValidator _updateValidator;

	public CategoryService(
		IApplicationDbContext ctx,
		CategoryMapper mapper,
		CreateCategoryValidator createValidator,
		UpdateCategoryValidator updateValidator
	)
	{
		_ctx = ctx;
		_mapper = mapper;
		_createValidator = createValidator;
		_updateValidator = updateValidator;
	}

	public async Task<Result<CategoryResponse>> CreateAsync(CreateCategoryCommand request)
	{
		var validationResult = await _createValidator.ValidateAsync(request);
		if (!validationResult.IsValid)
		{
			return validationResult.ToValidationError();
		}

		var category = _mapper.ToCategory(request);
		await _ctx.Categories.AddAsync(category);
		var res = await _ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return new UnknownError("Failed to create category.");
		}
		return _mapper.ToCategoryResponse(category);
	}

	public async Task<Result> DeleteAsync(int id)
	{
		var category = await _ctx.Categories.FindAsync(id);
		if (category == null)
		{
			return new NotFoundError("Category not found.");
		}
		_ctx.Categories.Remove(category);
		var result = await _ctx.SaveChangesAsync(default);
		if (result == 0)
		{
			return new UnknownError("Failed to delete category.");
		}
		return Result.Ok();
	}

	public async Task<List<CategoryResponse>> ListAsync()
	{
		var categories = await _ctx.Categories.ToListAsync();
		return [.. categories.Select(_mapper.ToCategoryResponse)];
	}

	public async Task<Result<CategoryResponse>> GetByIDAsync(int id)
	{
		var category = await _ctx.Categories.FindAsync(id);
		if (category == null)
		{
			return new NotFoundError("Category not found.");
		}
		return _mapper.ToCategoryResponse(category);
	}

	public async Task<List<HierarchyCategoryResponse>> ListHierarchyAsync()
	{
		var categories = await _ctx.Categories.AsNoTracking().ToListAsync();
		var responseDict = categories.ToDictionary(c => c.ID, _mapper.ToHierarchyCategoryResponse);

		var roots = new List<HierarchyCategoryResponse>();

		foreach (var category in categories)
		{
			var response = responseDict[category.ID];
			if (
				category.ParentCategoryID.HasValue
				&& responseDict.TryGetValue(category.ParentCategoryID.Value, out var parent)
			)
			{
				parent.Children.Add(response);
			}
			else
			{
				roots.Add(response);
			}
		}

		return roots;
	}

	public async Task<Result<CategoryResponse>> UpdateAsync(int id, UpdateCategoryCommand request)
	{
		var category = await _ctx.Categories.FindAsync(id);
		if (category == null)
		{
			return new NotFoundError("Category not found.");
		}

		var validationResult = await _updateValidator.ValidateAsync(request);
		if (!validationResult.IsValid)
		{
			return validationResult.ToValidationError();
		}

		_mapper.PatchCategory(request, category);
		_ctx.Categories.Update(category);
		var res = await _ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return new UnknownError("Failed to update category.");
		}

		return _mapper.ToCategoryResponse(category);
	}
}
