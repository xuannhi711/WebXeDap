using WebXeDap.Models;

namespace WebXeDap.Repositories
{
    public interface IHoaDonRepository
    {
        Task<IEnumerable<Hoadon>> GetAllAsync();
        Task<Hoadon> GetByIdAsync(int maHD);
        Task AddAsync(Hoadon hoadon);
        Task UpdateAsync(Hoadon hoadon);
    }
}
