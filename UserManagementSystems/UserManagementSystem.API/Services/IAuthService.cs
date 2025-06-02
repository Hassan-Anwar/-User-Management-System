
using System.Threading.Tasks;
using UserManagementSystem.API.DTOs;
using UserManagementSystem.API.Models;

namespace UserManagementSystem.API.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }
}
