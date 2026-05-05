using Ardalis.Specification;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Contracts.Persistence;

public interface IProductRepository : IRepositoryBase<Product>
{
	Task<IEnumerable<Product>> GetProductsByCategoryIDAsync(int categoryID);
	// Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
}
