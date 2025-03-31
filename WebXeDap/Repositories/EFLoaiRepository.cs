using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;

namespace WebXeDap.Repositories
{
    public class EFLoaiRepository : ILoaiRepository
    {
        private readonly ApplicationDbContext _context;

        public EFLoaiRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Loai>> GetAllAsync()
        {
            return await _context.Loais.ToListAsync(); 
  

        }

        public async Task<Loai> GetByIdAsync(string id)
        {
             return await _context.Loais.FindAsync(id);
            // lấy thông tin kèm theo category 

        }

        public async Task AddAsync(Loai product)
        {

            _context.Loais.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Loai  product)
        {
            _context.Loais.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var product = await _context.Loais.FindAsync(id);
            _context.Loais.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
