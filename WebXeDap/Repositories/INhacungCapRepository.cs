using WebXeDap.Models;

namespace WebXeDap.Repositories
{
    public interface INhacungcapRepository
    {
        Task<IEnumerable<Nhacungcap>> GetAllAsync();
        Task<Nhacungcap> GetByIdAsync(string maNCC);
        Task AddAsync(Nhacungcap nhacungcap);
        Task UpdateAsync(Nhacungcap nhacungcap);
        Task DeleteAsync(string maNCC);
    }

}
