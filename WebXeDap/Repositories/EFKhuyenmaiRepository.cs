using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;

namespace WebXeDap.Repositories
{
    public class EFKhuyenmaiRepository : IKhuyenmaiRepository
    {
        private readonly ApplicationDbContext _context;

        public EFKhuyenmaiRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Khuyenmai>> GetAllAsync()
        {
            return await _context.Khuyenmais.ToListAsync();
        }

        public async Task<Khuyenmai> GetByIdAsync(string maKM)
        {
            return await _context.Khuyenmais.FindAsync(maKM);
        }

        public async Task AddAsync(Khuyenmai khuyenmai)
        {
            _context.Khuyenmais.Add(khuyenmai);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Khuyenmai khuyenmai)
        {
            _context.Khuyenmais.Update(khuyenmai);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string maKM)
        {
            var km = await _context.Khuyenmais.FindAsync(maKM);
            if (km != null)
            {
                _context.Khuyenmais.Remove(km);
                await _context.SaveChangesAsync();
            }
        }
    }

}
