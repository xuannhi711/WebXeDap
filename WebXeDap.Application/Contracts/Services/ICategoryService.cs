using WebXeDap.Application.Features.Catalog.DTOs;

namespace WebXeDap.Application.Contracts.Services;

public interface ICategoryService
{
	Task<CategoryResponse?> GetByIDAsync(int id);

	Task<List<CategoryResponse>> ListAsync();

	Task<List<HierarchyCategoryResponse>> ListHierarchyAsync();

	Task<CategoryResponse?> CreateAsync(CreateCategoryCommand request);

	Task<CategoryResponse?> UpdateAsync(UpdateCategoryCommand request);

	Task<bool> DeleteAsync(int id);
}
