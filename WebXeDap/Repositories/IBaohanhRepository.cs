using WebXeDap.Models;

namespace WebXeDap.Repositories
{
	public interface IBaohanhRepository
	{
		Task AddAsync(Baohanh baohanh);
		Task<IEnumerable<Baohanh>> GetAllAsync();
		Task<IEnumerable<Baohanh>> GetByUserIdAsync(string userId);
		Task<IEnumerable<Baohanh>> GetByMaBaoHanhAsync(string maBaoHanh);
	}
}
