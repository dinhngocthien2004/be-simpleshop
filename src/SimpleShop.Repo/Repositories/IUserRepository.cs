using SimpleShop.Repo.Models;
namespace SimpleShop.Repo.Repositories;

public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(int id);
    Task<AppUser?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task AddAsync(AppUser user);
    Task SaveChangesAsync();
}
