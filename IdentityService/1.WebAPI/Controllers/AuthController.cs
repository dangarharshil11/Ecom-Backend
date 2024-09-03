using IdentityService._2.BusinessLogic.DTO;
using IdentityService._2.BusinessLogic.IBusinessLogic;
using IdentityService._4.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService._1.WebAPI.Controllers
{
    [Route("api/identity/auth")]
    [ApiController]
    public class AuthController(IAuthLogic authLogic) : ControllerBase
    {
        private readonly IAuthLogic _authLogic = authLogic;

        [HttpPost]
        [Route("register")]
        public async Task<CommonResponse> Register([FromBody] RegisterRequestDto registrationRequest)
        {
            return await _authLogic.Register(registrationRequest);
        }

        [HttpPost]
        [Route("login")]
        public async Task<CommonResponse> Login([FromBody] LoginRequestDto loginRequest)
        {
            return await _authLogic.Login(loginRequest);
        }

        [HttpPost]
        [Route("changePassword")]
        [Authorize]
        public async Task<CommonResponse> ChangePassword(ChangePasswordRequestDto changePasswordRequest)
        {
            return await _authLogic.ChangePassword(changePasswordRequest);
        }

        [HttpGet]
        [Route("forgotPassword/{email}")]
        public async Task<CommonResponse> ForgotPassword([FromRoute]string email)
        {
            return await _authLogic.ForgotPassword(email);
        }
    }
}
