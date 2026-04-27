using Microsoft.AspNetCore.Identity;
using WebXeDap.Models;

namespace WebXeDap.Repositories
{
	public interface INguoiDungRepository
	{
		Task<IEnumerable<ApplicationUser>> GetAllAsync();
		Task<ApplicationUser> GetByIdAsync(string id);
		Task<ApplicationUser> GetByEmailAsync(string email);
		Task AddAsync(ApplicationUser user);
		Task UpdateAsync(ApplicationUser user);
		Task DeleteAsync(string id);
	}
}
