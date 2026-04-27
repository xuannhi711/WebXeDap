namespace WebXeDap.Repositories
{
	using WebXeDap.Models;

	public interface ISanphamRepository
	{
		Task<IEnumerable<Sanpham>> GetAllAsync();
		Task<Sanpham> GetByIdAsync(string maSP);
		Task AddAsync(Sanpham sanpham);
		Task UpdateAsync(Sanpham sanpham);
		Task DeleteAsync(string maSP);
		Task AddAnhAsync(Anh Image);
		Task AddMauAsync(Mau mau);
	}
}
