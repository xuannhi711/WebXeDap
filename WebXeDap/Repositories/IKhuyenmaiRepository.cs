using WebXeDap.Models;

namespace WebXeDap.Repositories
{
	public interface IKhuyenmaiRepository
	{
		Task<IEnumerable<Khuyenmai>> GetAllAsync();
		Task<Khuyenmai> GetByIdAsync(string maKM);
		Task AddAsync(Khuyenmai khuyenmai);
		Task UpdateAsync(Khuyenmai khuyenmai);
		Task DeleteAsync(string maKM);
	}
}
