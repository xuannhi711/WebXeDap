using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;

namespace WebXeDap.Repositories
{
    public class BaohanhRepository : IBaohanhRepository
    {
        private readonly ApplicationDbContext _context;private readonly IHoaDonRepository _hoaDonRepo;

        public BaohanhRepository(ApplicationDbContext context, IHoaDonRepository hoaDonRepo)
        {
            _context = context; _hoaDonRepo = hoaDonRepo;
        }
        public async Task<IEnumerable<Baohanh>> GetAllAsync()
        {
            return await _context.Baohanhs.Include(b => b.Hoadon).ToListAsync();
        }
        public async Task AddAsync(Baohanh baohanh)
        {
            await _context.Baohanhs.AddAsync(baohanh);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Baohanh>> GetByUserIdAsync(string userId)
        {
            return await _context.Baohanhs
                .Include(bh => bh.Hoadon)
                .Where(bh => bh.Hoadon.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Baohanh>> GetByMaBaoHanhAsync(string maBaoHanh)
        {
            return await _context.Baohanhs
                .Include(b => b.Hoadon)
                .Where(b => b.MaBaoHanh.ToString().Contains(maBaoHanh))
                .ToListAsync();
        }
    }
}
