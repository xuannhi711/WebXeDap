using WebXeDap.Application.Features.Catalog.DTOs;

namespace WebXeDap.Application.Contracts.Services;

public interface ICategoryService
{
	Task<CategoryResponse?> GetByIDAsync(int id, CancellationToken ct = default);
	Task<List<CategoryResponse>> GetAllAsync(CancellationToken ct = default);
	Task<List<HierarchyCategoryResponse>> GetHierarchyAsync(CancellationToken ct = default);
	Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default);
	Task<int> UpdateAsync(UpdateCategoryRequest request, CancellationToken ct = default);
	Task<bool> DeleteAsync(DeleteCategoryRequest request, CancellationToken ct = default);
}
