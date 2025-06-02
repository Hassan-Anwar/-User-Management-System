
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagementSystem.API.Models;

namespace UserManagementSystem.API.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(Guid id);
        Task<List<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);
        Task<List<User>> GetRecentLoginsAsync(TimeSpan withinTime);
    }
}
