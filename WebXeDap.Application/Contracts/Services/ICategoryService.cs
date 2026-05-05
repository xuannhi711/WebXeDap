using WebXeDap.Application.DTOs;

namespace WebXeDap.Application.Contracts.Services;

public interface ICategoryService
{
	Task<CategoryResponse?> GetByIDAsync(int id, CancellationToken ct = default);
	Task<List<CategoryResponse>> ListAsync(CancellationToken ct = default);
	Task<List<HierarchyCategoryResponse>> ListHierarchyAsync(CancellationToken ct = default);
	Task<CategoryResponse> CreateAsync(
		CreateCategoryRequest request,
		CancellationToken ct = default
	);
	Task<int> UpdateAsync(UpdateCategoryRequest request, CancellationToken ct = default);
	Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
