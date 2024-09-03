using IdentityService._3.DataAccess.Domains;
using Microsoft.AspNetCore.Identity;

namespace IdentityService._3.DataAccess.IRepositories
{
    public interface IUserRepository
    {
        Task<IdentityResult> DeleteUser(ApplicationUser user);
        Task<List<ApplicationUser>> GetAll();
        Task<ApplicationUser?> GetUserById(string Id);
        Task<ApplicationUser> UpdateUser(ApplicationUser user, ApplicationUser updatedUser);
    }
}
