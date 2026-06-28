using Microsoft.EntityFrameworkCore;
using SimpleShop.Repo.Data;
using SimpleShop.Repo.Models;

namespace SimpleShop.Repo.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SimpleShopDbContext _context;
    public UserRepository(SimpleShopDbContext context) => _context = context;

    public Task<AppUser?> GetByIdAsync(int id) => _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    public Task<AppUser?> GetByEmailAsync(string email) => _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    public Task<bool> EmailExistsAsync(string email) => _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    public Task AddAsync(AppUser user) => _context.Users.AddAsync(user).AsTask();
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
