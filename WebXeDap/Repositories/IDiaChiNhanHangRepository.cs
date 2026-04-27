using WebXeDap.Models;

namespace WebXeDap.Repositories
{
	public interface IDiaChiNhanHangRepository
	{
		List<DiaChiNhanHang> GetAll();
		DiaChiNhanHang GetById(int id);
		void Add(DiaChiNhanHang diaChi);
		void Update(DiaChiNhanHang diaChi);
		void Delete(int id);
	}
}
