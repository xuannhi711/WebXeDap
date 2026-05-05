using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog.DTOs;
using WebXeDap.Application.Features.Catalog.Mapper;

namespace WebXeDap.Application.Features.Catalog;

public class CategoryService : ICategoryService
{
	private readonly ICategoryRepository _categoryRepo;
	private readonly CategoryMapper _mapper;

	public CategoryService(
		ICategoryRepository categoryRepo,
		CategoryMapper mapper
	)
	{
		_categoryRepo = categoryRepo;
		_mapper = mapper;
	}

	public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default)
	{
		var category = _mapper.CategoryCreateRequestToCategory(request);
		var newCategory = await _categoryRepo.AddAsync(category, ct);
		return _mapper.CategoryToCategoryResponse(newCategory);
	}

	public async Task<bool> DeleteAsync(DeleteCategoryRequest request, CancellationToken ct = default)
	{
		var category = _mapper.CategoryDeleteRequestToCategory(request);
		return (await _categoryRepo.DeleteAsync(category, ct)) == 1;
	}

	public async Task<List<CategoryResponse>> GetAllAsync(CancellationToken ct = default)
	{
		var categories = await _categoryRepo.ListAsync(ct);
		return [.. categories.Select(_mapper.CategoryToCategoryResponse)];
	}

	public async Task<CategoryResponse?> GetByIDAsync(int id, CancellationToken ct = default)
	{
		var category = await _categoryRepo.GetByIdAsync(id, ct);
		if (category == null)
		{
			return null;
		}
		return _mapper.CategoryToCategoryResponse(category);
	}

	public async Task<List<HierarchyCategoryResponse>> GetHierarchyAsync(CancellationToken ct = default)
	{
		var categories = await _categoryRepo.ListAsync(ct);
		var responseDict = categories.ToDictionary(
			c => c.ID,
			_mapper.CategoryToHierarchyCategoryResponse);

		var roots = new List<HierarchyCategoryResponse>();

		foreach (var category in categories)
		{
			var response = responseDict[category.ID];
			if (category.ParentCategoryID.HasValue &&
				responseDict.TryGetValue(category.ParentCategoryID.Value, out var parent))
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

	public async Task<int> UpdateAsync(UpdateCategoryRequest request, CancellationToken ct = default)
	{
		var category = _mapper.CategoryUpdateRequestToCategory(request);
		return await _categoryRepo.UpdateAsync(category, ct);
	}
}
