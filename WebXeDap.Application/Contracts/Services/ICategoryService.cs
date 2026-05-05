using WebXeDap.Application.Features.Catalog.DTOs;

namespace WebXeDap.Application.Contracts.Services;

public interface ICategoryService
{
	Task<CategoryResponse?> GetByIDAsync(int id, CancellationToken ct = default);
	Task<List<CategoryResponse>> GetAllAsync(CancellationToken ct = default);
	Task<List<CategoryResponse>> GetHierarchyAsync(CancellationToken ct = default);
	Task<int> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default);
	Task UpdateAsync(UpdateCategoryRequest request, CancellationToken ct = default);
	Task DeleteAsync(DeleteCategoryRequest request, CancellationToken ct = default);
}
