using CustomerService._2.BusinessLogic.DTO;

namespace CustomerService._1.WebAPI.HelperServices.IServices
{
    public interface IIdentityService
    {
        Task<UserDTO?> GetUser(Guid userId);
    }
}