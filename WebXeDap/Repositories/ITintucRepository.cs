using WebXeDap.Models;
namespace WebXeDap.Repositories
{
   
    public interface ITintucRepository
    {
        Task<IEnumerable<tintuc>> GetAllAsync();
        Task<tintuc> GetByIdAsync(int id);
        Task AddAsync(tintuc tintuc);
        Task UpdateAsync(tintuc tintuc);
        Task DeleteAsync(int id);
    }
}

