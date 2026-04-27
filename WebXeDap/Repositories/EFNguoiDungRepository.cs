using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;

namespace WebXeDap.Repositories
{
	public class EFNguoiDungRepository : INguoiDungRepository
	{
		private readonly ApplicationDbContext _context;

		public EFNguoiDungRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
		{
			return await _context.Users.ToListAsync();
		}

		public async Task<ApplicationUser> GetByIdAsync(string id)
		{
			return await _context.Users.FindAsync(id);
		}

		public async Task AddAsync(ApplicationUser user)
		{
			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(ApplicationUser user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(string id)
		{
			var user = await _context.Users.FindAsync(id);
			if (user != null)
			{
				_context.Users.Remove(user);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<ApplicationUser> GetByEmailAsync(string email)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}
	}
}
