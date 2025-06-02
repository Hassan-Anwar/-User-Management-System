
using System;

namespace UserManagementSystem.API.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
