
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.API.Models;

namespace UserManagementSystem.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public Task<User> GetByEmailAsync(string email) =>
            _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public Task<User> GetByIdAsync(Guid id) =>
            _context.Users.FindAsync(id).AsTask();

        public Task<List<User>> GetAllAsync() =>
            _context.Users.ToListAsync();

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public Task<List<User>> GetRecentLoginsAsync(TimeSpan withinTime)
        {
            var threshold = DateTime.UtcNow.Subtract(withinTime);
            return _context.Users
                .Where(u => u.LastLoginAt != null && u.LastLoginAt > threshold)
                .ToListAsync();
        }
    }
}
