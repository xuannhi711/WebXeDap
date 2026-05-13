using Util.Primitives.ResultType;
using WebXeDap.Application.Features.Catalog.DTOs;

namespace WebXeDap.Application.Contracts.Services;

public interface IProductService
{
	Task<Result<SimpleProductResponse>> CreateAsync(CreateProductCommand cmd);

	Task<Result<DetailedProductResponse>> GetByIDAsync(int id);

	Task<List<SimpleProductResponse>> FilterAsync(FilterProductCommand cmd, int page, int size);

	Task<int> CountAsync(FilterProductCommand cmd);

	Task<Result<DetailedProductResponse>> UpdateAsync(UpdateProductCommand cmd);

	Task<Result> DeleteAsync(int id);
}
