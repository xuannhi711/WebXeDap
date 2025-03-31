using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;

namespace WebXeDap.Repositories
{
    public class EFHoaDonRepository : IHoaDonRepository
    {
        private readonly ApplicationDbContext _context;

        public EFHoaDonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Hoadon hoadon)
        {
            await _context.Hoadon.AddAsync(hoadon);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Hoadon>> GetAllAsync()
        {
            return await _context.Hoadon.Include(h => h.ApplicationUser)
                                         .Include(h => h.Chitiethoadons)
                                         .ThenInclude(ct => ct.Sanpham)
                                         .ToListAsync();
        }

        public async Task<Hoadon> GetByIdAsync(int maHD)
        {
            return await _context.Hoadon.Include(h => h.ApplicationUser)
                                         .Include(h => h.Chitiethoadons)
                                         .ThenInclude(ct => ct.Sanpham)
                                         .FirstOrDefaultAsync(h => h.MaHD == maHD);
        }

        public async Task UpdateAsync(Hoadon hoadon)
        {
            _context.Hoadon.Update(hoadon);
            await _context.SaveChangesAsync();
        }
    }
}
