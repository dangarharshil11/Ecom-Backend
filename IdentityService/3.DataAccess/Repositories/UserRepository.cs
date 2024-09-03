using IdentityService._3.DataAccess.Context;
using IdentityService._3.DataAccess.Domains;
using IdentityService._3.DataAccess.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService._3.DataAccess.Repositories
{
    public class UserRepository(UserManager<ApplicationUser> userManager, IdentityDbContext dbContext) : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IdentityDbContext _dbContext = dbContext;

        public async Task<List<ApplicationUser>> GetAll()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserById(string Id)
        {
            return await _userManager.FindByIdAsync(Id);
        }

        public async Task<IdentityResult> DeleteUser(ApplicationUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<ApplicationUser> UpdateUser(ApplicationUser user, ApplicationUser updatedUser)
        {
            _dbContext.Entry(user).CurrentValues.SetValues(updatedUser);
            await _dbContext.SaveChangesAsync();
            return updatedUser;
        }
    }
}
