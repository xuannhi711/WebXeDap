using WebXeDap.Application.Contracts.Pagination;
using WebXeDap.Application.Features.Catalog.DTOs;

namespace WebXeDap.Application.Contracts.Services;

public interface IProductService
{
	Task<SimpleProductResponse?> CreateAsync(CreateProductRequest req);

	Task<DetailedProductResponse?> GetByIDAsync(int id);

	Task<PaginatedResult<SimpleProductResponse>> FilterAsync(FilterProductRequest req);

	Task<DetailedProductResponse?> UpdateAsync(UpdateProductRequest req);

	Task<bool> DeleteAsync(int id);
}
