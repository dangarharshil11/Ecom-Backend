using IdentityService._3.DataAccess.Domains;
using IdentityService._4.Infrastructure.Models;

namespace IdentityService._2.BusinessLogic.IBusinessLogic
{
    public interface IUserLogic
    {
        Task<CommonResponse> DeleteUser(string Id);
        Task<CommonResponse> GetAllUsers();
        Task<CommonResponse> GetUserById(string Id);
        Task<CommonResponse> UpdateUser(string Id, ApplicationUser updatedUser);
    }
}
