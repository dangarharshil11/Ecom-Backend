using IdentityService._2.BusinessLogic.DTO;
using IdentityService._2.BusinessLogic.IBusinessLogic;
using IdentityService._3.DataAccess.Domains;
using IdentityService._4.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService._1.WebAPI.Controllers
{
    [Route("api/identity/user")]
    [ApiController]
    public class UserController(IAuthLogic authLogic, IUserLogic userLogic) : ControllerBase
    {
        private readonly IAuthLogic _authLogic = authLogic;
        private readonly IUserLogic _userLogic = userLogic;

        [HttpPost]
        [Route("createUser")]
        [Authorize(Roles = "Admin")]
        public async Task<CommonResponse> CreateUser([FromBody] RegisterRequestDto registerRequest)
        {
            return await _authLogic.Register(registerRequest);
        }

        [HttpGet]
        [Route("getUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<CommonResponse> GetUsers()
        {
            return await _userLogic.GetAllUsers();
        }

        [HttpGet]
        [Route("getUserById/{Id}")]
        [Authorize]
        public async Task<CommonResponse> GetUserById(string Id)
        {
            return await _userLogic.GetUserById(Id);
        }

        [HttpDelete]
        [Route("DeleteUser/{Id}")]
        [Authorize]
        public async Task<CommonResponse> DeleteUser(string Id)
        {
            return await _userLogic.DeleteUser(Id);
        }

        [HttpPost]
        [Route("UpdateUser/{Id}")]
        [Authorize]
        public async Task<CommonResponse> UpdateUser(string Id, [FromBody] ApplicationUser updatedUser)
        {
            return await _userLogic.UpdateUser(Id, updatedUser);
        }
    }
}
