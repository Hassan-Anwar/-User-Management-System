
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using UserManagementSystem.API.DTOs;
using UserManagementSystem.API.Models;
using UserManagementSystem.API.Repositories;
using UserManagementSystem.API.Webhooks;

namespace UserManagementSystem.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<User> _hasher;
        private readonly IWebhookService _webhook;

        public AuthService(IUserRepository repository, IConfiguration config, IWebhookService webhook)
        {
            _repository = repository;
            _config = config;
            _hasher = new PasswordHasher<User>();
            _webhook = webhook;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);
            await _repository.AddAsync(user);

            Log.Information("User registered: {Email}", user.Email);
            return GenerateToken(user);
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _repository.GetByEmailAsync(dto.Email);
            if (user == null || _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) != PasswordVerificationResult.Success)
            {
                Log.Warning("Failed login attempt: {Email}", dto.Email);
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _repository.UpdateAsync(user);
            await _webhook.SendLoginWebhookAsync();

            Log.Information("User logged in: {Email}", user.Email);
            return GenerateToken(user);
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
