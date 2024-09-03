using IdentityService._3.DataAccess.Domains;
using IdentityService._3.DataAccess.IRepositories;
using Microsoft.AspNetCore.Identity;

namespace IdentityService._3.DataAccess.Repositories
{
    public class AuthRepository(UserManager<ApplicationUser> userManager) : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<ApplicationUser?> GetUserByEmail(string email)
        {
            ApplicationUser? User = await _userManager.FindByEmailAsync(email);
            return User;
        }

        public async Task<IdentityResult> CreateUser(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> AssignRole(ApplicationUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<List<string>> GetRolesByUser(ApplicationUser user)
        {
            if (user == null || user.Email == null)
            {
                return new List<string>();
            }
            else
            {
                ApplicationUser? existingUser = await GetUserByEmail(user.Email);
                if (existingUser == null)
                {
                    return new List<string>();
                }
                else
                {
                    return (List<string>)await _userManager.GetRolesAsync(user);
                }
            }
        }

        public async Task<bool> CheckPassword(ApplicationUser user, string password)
        {
            if (user == null || user.Email == null || password == null)
            {
                return false;
            }
            else
            {
                ApplicationUser? existingUser = await GetUserByEmail(user.Email);
                if (existingUser == null)
                {
                    return false;
                }
                else
                {
                    return await _userManager.CheckPasswordAsync(existingUser, password);
                }
            }
        }

        public async Task<IdentityResult> ChangePassword(ApplicationUser user, string newPassword)
        {
            // Changing Password to newly entered password
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            IdentityResult identityResult = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return identityResult;
        }

    }
}
