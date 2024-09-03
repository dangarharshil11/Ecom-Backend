using IdentityService._2.BusinessLogic.DTO;
using IdentityService._4.Infrastructure.Models;

namespace IdentityService._2.BusinessLogic.IBusinessLogic
{
    public interface IAuthLogic
    {
        Task<CommonResponse> ChangePassword(ChangePasswordRequestDto changePasswordRequest);
        Task<CommonResponse> ForgotPassword(string email);
        Task<CommonResponse> Login(LoginRequestDto loginRequest);
        Task<CommonResponse> Register(RegisterRequestDto registerRequest);
    }
}
