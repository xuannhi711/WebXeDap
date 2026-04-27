using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;

namespace WebXeDap.Repositories
{
	public class TintucRepository : ITintucRepository
	{
		private readonly ApplicationDbContext _context;

		public TintucRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<tintuc>> GetAllAsync()
		{
			return await _context.tintuc.ToListAsync();
		}

		public async Task<tintuc> GetByIdAsync(int id)
		{
			return await _context.tintuc.FindAsync(id);
		}

		public async Task AddAsync(tintuc tintuc)
		{
			_context.tintuc.Add(tintuc);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(tintuc tintuc)
		{
			_context.tintuc.Update(tintuc);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var tintuc = await _context.tintuc.FindAsync(id);
			if (tintuc != null)
			{
				_context.tintuc.Remove(tintuc);
				await _context.SaveChangesAsync();
			}
		}
	}
}
