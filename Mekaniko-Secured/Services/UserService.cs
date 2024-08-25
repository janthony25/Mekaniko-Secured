using Mekaniko_Secured.Data;
using Mekaniko_Secured.Models;
using Mekaniko_Secured.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mekaniko_Secured.Services
{
    public class UserService
    {
        private readonly DataContext _data;
        public UserService(DataContext data)
        {
            _data = data;
        }

        public async Task<(bool success, string message)> AuthenticateUser(LoginDto loginDto)
        {
            var user = await _data.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
                return (false, "Username not found");

            if (BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return (true, "Authentication Successful");

            return (false, "Invalid password");
        }
        
        public async Task<User> GetUserByUsername(string username)
        {
            return await _data.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        // New method for changing password
        public async Task<(bool success, string message)> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            // Find the user by username
            var user = await _data.Users.FirstOrDefaultAsync(u => u.Username == changePasswordDto.Username);

            if (user == null)
                return (false, "User not found");

            // Verift the current password
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
                return (false, "Current password is incorrect");

            // Has the new password and update the user
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _data.SaveChangesAsync();

            return (true, "Password changed successfully!");
        }
    }
}
