using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;

namespace WebXeDap.Repositories
{

    public class EFSanphamRepository : ISanphamRepository
    {
        private readonly ApplicationDbContext _context;

        public EFSanphamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sanpham>> GetAllAsync()
        {
            return await _context.Sanphams
                .Include(s => s.Loai)
                .Include(sp => sp.Mau)
                .Include(s => s.Nhacungcap)
                .Include(s => s.Khuyenmai)
                .Include(s => s.Anhs)
                .ToListAsync();
        }

        public async Task<Sanpham> GetByIdAsync(string maSP)
        {
            return await _context.Sanphams
                .Include(s => s.Loai)
                .Include(sp => sp.Mau)
                .Include(s => s.Nhacungcap)
                .Include(s => s.Khuyenmai)
                .Include(s => s.Anhs)
                .FirstOrDefaultAsync(s => s.MaSP == maSP);
        }

        public async Task AddAsync(Sanpham sanpham)
        {
            _context.Sanphams.Add(sanpham);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Sanpham sanpham)
        {
            _context.Sanphams.Update(sanpham);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string maSP)
        {
            var sanpham = await _context.Sanphams.FindAsync(maSP);
            if (sanpham != null)
            {
                _context.Sanphams.Remove(sanpham);
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddAnhAsync(Anh Image)
        {
            _context.Anhs.Add(Image);
            await _context.SaveChangesAsync();
        }
    }
}