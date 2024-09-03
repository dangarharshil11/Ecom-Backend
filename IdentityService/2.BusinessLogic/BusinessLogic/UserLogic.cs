using Azure;
using IdentityService._2.BusinessLogic.IBusinessLogic;
using IdentityService._3.DataAccess.Domains;
using IdentityService._3.DataAccess.IRepositories;
using IdentityService._4.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService._2.BusinessLogic.BusinessLogic
{
    public class UserLogic(IUserRepository userRepository) : IUserLogic
    {
        private readonly IUserRepository _userRepository = userRepository;
        protected CommonResponse _response = new();
        public async Task<CommonResponse> GetAllUsers()
        {
            List<ApplicationUser> usersList = await _userRepository.GetAll();

            _response.IsSuccess = true;
            _response.Data = usersList;
            return _response;
        }

        public async Task<CommonResponse> GetUserById(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                _response.Message = "User Id is null or Empty";
            }
            else
            {
                ApplicationUser? user = await _userRepository.GetUserById(Id);

                if (user == null)
                {
                    _response.Message = "User Not Found.";
                }
                else
                {
                    _response.IsSuccess = true;
                    _response.Data = user;
                }
            }
            return _response;
        }

        public async Task<CommonResponse> DeleteUser(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                _response.Message = "User Id is null or Empty";
            }
            else
            {
                ApplicationUser? user = await _userRepository.GetUserById(Id);

                if (user == null)
                {
                    _response.Message = "User Not Found.";
                }
                else
                {
                    IdentityResult deleteUserResult = await _userRepository.DeleteUser(user);
                    if (deleteUserResult.Succeeded)
                    {
                        _response.IsSuccess = true;
                        _response.Message = "User Deleted Successfully.";
                        _response.Data = user;
                    }
                    // If Assigning Role to user fails
                    else
                    {
                        if (deleteUserResult.Errors.Any())
                        {
                            foreach (var error in deleteUserResult.Errors)
                            {
                                _response.Message = error.Description;
                            }
                        }
                    }
                }
            }
            return _response;
        }

        public async Task<CommonResponse> UpdateUser(string Id, ApplicationUser updatedUser)
        {
            if (string.IsNullOrEmpty(Id))
            {
                _response.Message = "User Id is null or Empty";
            }
            else
            {
                ApplicationUser? user = await _userRepository.GetUserById(Id);

                if (user == null)
                {
                    _response.Message = "User Not Found.";
                }
                else if(
                    user.FirstName != updatedUser.FirstName ||
                    user.LastName != updatedUser.LastName ||
                    user.Address != updatedUser.Address ||
                    user.PhoneNumber != updatedUser.PhoneNumber
                    )
                {
                    user = await _userRepository.UpdateUser(user, updatedUser);
                    _response.IsSuccess = true;
                    _response.Data = user;
                    _response.Message = "User Updated Successfully.";
                }
                else
                {
                    _response.IsSuccess = true;
                    _response.Data = user;
                    _response.Message = "User Updated Successfully.";
                }
            }
            return _response;
        }
    }
}
