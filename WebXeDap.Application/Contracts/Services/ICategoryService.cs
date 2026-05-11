using WebXeDap.Application.Features.Catalog.DTOs;

namespace WebXeDap.Application.Contracts.Services;

public interface ICategoryService
{
	Task<CategoryResponse?> GetByIDAsync(int id);

	Task<List<CategoryResponse>> ListAsync();

	Task<List<HierarchyCategoryResponse>> ListHierarchyAsync();

	Task<CategoryResponse?> CreateAsync(CreateCategoryRequest request);

	Task<CategoryResponse?> UpdateAsync(UpdateCategoryRequest request);

	Task<bool> DeleteAsync(int id);
}
