using IdentityService._3.DataAccess.Domains;
using Microsoft.AspNetCore.Identity;

namespace IdentityService._3.DataAccess.IRepositories
{
    public interface IAuthRepository
    {
        Task<IdentityResult> AssignRole(ApplicationUser user, string role);
        Task<IdentityResult> ChangePassword(ApplicationUser user, string newPassword);
        Task<bool> CheckPassword(ApplicationUser user, string password);
        Task<IdentityResult> CreateUser(ApplicationUser user, string password);
        Task<List<string>> GetRolesByUser(ApplicationUser user);
        Task<ApplicationUser?> GetUserByEmail(string email);
    }
}
