using WebXeDap.Application.DTOs;

namespace WebXeDap.Application.Contracts.Services;

public interface IProductService
{
	Task<ProductResponse> CreateAsync(CreateProductRequest req, CancellationToken ct = default);

	Task<ProductResponse?> GetByIDAsync(int id, CancellationToken ct = default);

	Task<PaginatedResult<ProductResponse>> ListAsync(CancellationToken ct = default);

	Task<PaginatedResult<ProductResponse>> FilterAsync(
		FilterProductRequest req,
		CancellationToken ct = default
	);

	Task<int> UpdateAsync(UpdateProductRequest req, CancellationToken ct = default);

	Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
