using Util.Primitives.ResultType;
using WebXeDap.Application.Features.Catalog.DTOs;

namespace WebXeDap.Application.Contracts.Services;

public interface ICategoryService
{
	Task<Result<CategoryResponse>> GetByIDAsync(int id);

	Task<List<CategoryResponse>> ListAsync();

	Task<List<HierarchyCategoryResponse>> ListHierarchyAsync();

	Task<Result<CategoryResponse>> CreateAsync(CreateCategoryCommand request);

	Task<Result<CategoryResponse>> UpdateAsync(UpdateCategoryCommand request);

	Task<Result> DeleteAsync(int id);
}
