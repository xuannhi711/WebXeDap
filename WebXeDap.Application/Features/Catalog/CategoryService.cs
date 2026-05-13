using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Features.Catalog.Mapper;
using WebXeDap.Application.Features.Catalog.Validators;

namespace WebXeDap.Application.Features.Catalog;

public class CategoryService : ICategoryService
{
	private readonly IApplicationDbContext _ctx;
	private readonly CategoryMapper _mapper;
	private readonly CreateCategoryValidator _createValidator;
	private readonly UpdateCategoryValidator _updateValidator;
	private readonly DeleteCategoryValidator _deleteValidator;

	public CategoryService(
		IApplicationDbContext ctx,
		CategoryMapper mapper,
		CreateCategoryValidator createValidator,
		UpdateCategoryValidator updateValidator,
		DeleteCategoryValidator deleteValidator
	)
	{
		_ctx = ctx;
		_mapper = mapper;
		_createValidator = createValidator;
		_updateValidator = updateValidator;
		_deleteValidator = deleteValidator;
	}

	public async Task<CategoryResponse?> CreateAsync(CreateCategoryCommand request)
	{
		var validationResult = await _createValidator.ValidateAsync(request);
		if (!validationResult.IsValid)
		{
			return null;
		}

		var category = _mapper.ToCategory(request);
		await _ctx.Categories.AddAsync(category);
		var res = await _ctx.SaveChangesAsync(default);
		if (res == 0)
		{
			return null;
		}
		return _mapper.ToCategoryResponse(category);
	}

	public async Task<bool> DeleteAsync(int id)
	{
		var validationResult = await _deleteValidator.ValidateAsync(id);
		if (!validationResult.IsValid)
		{
			return false;
		}
		var category = await _ctx.Categories.FindAsync(id);
		if (category == null)
		{
			return false;
		}
		_ctx.Categories.Remove(category);
		var result = await _ctx.SaveChangesAsync(default);
		return result == 1;
	}

	public async Task<List<CategoryResponse>> ListAsync()
	{
		var categories = await _ctx.Categories.AsNoTracking().ToListAsync();
		return [.. categories.Select(_mapper.ToCategoryResponse)];
	}

	public async Task<CategoryResponse?> GetByIDAsync(int id)
	{
		var category = await _ctx.Categories.FindAsync(id);
		if (category == null)
		{
			return null;
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

	public async Task<CategoryResponse?> UpdateAsync(UpdateCategoryCommand request)
	{
		var validationResult = await _updateValidator.ValidateAsync(request);
		if (!validationResult.IsValid)
		{
			return null;
		}

		var category = await _ctx.Categories.FirstOrDefaultAsync(x => x.ID == request.ID);

		if (category is null)
		{
			return null;
		}

		_mapper.PatchCategory(request, category);

		try
		{
			await _ctx.SaveChangesAsync(default);
		}
		catch (DbUpdateConcurrencyException)
		{
			return null;
		}

		return _mapper.ToCategoryResponse(category);
	}
}
