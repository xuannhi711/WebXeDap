
using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;
using WebXeDap.Repositories;

namespace WebXeDap.Repositories
{
    public class EFNhacungcapRepository : INhacungcapRepository
    {
        private readonly ApplicationDbContext _context;

        public EFNhacungcapRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Nhacungcap>> GetAllAsync()
        {
            return await _context.Nhacungcaps.ToListAsync();
        }

        public async Task<Nhacungcap> GetByIdAsync(string maNCC)
        {
            return await _context.Nhacungcaps.FirstOrDefaultAsync(n => n.MaNCC == maNCC);
        }

        public async Task AddAsync(Nhacungcap nhacungcap)
        {
            _context.Nhacungcaps.Add(nhacungcap);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Nhacungcap nhacungcap)
        {
            _context.Nhacungcaps.Update(nhacungcap);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string maNCC)
        {
            var ncc = await _context.Nhacungcaps.FindAsync(maNCC);
            if (ncc != null)
            {
                _context.Nhacungcaps.Remove(ncc);
                await _context.SaveChangesAsync();
            }
        }
    }
}
