using IdentityService._2.BusinessLogic.DTO;
using IdentityService._2.BusinessLogic.IBusinessLogic;
using IdentityService._3.DataAccess.Domains;
using IdentityService._3.DataAccess.IRepositories;
using IdentityService._4.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IdentityService._2.BusinessLogic.BusinessLogic
{
    public class AuthLogic(IAuthRepository authRepository, ITokenRepository tokenRepository, IConfiguration configuration) : IAuthLogic
    {
        protected CommonResponse _response = new();
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly ITokenRepository _tokenRepository = tokenRepository;
        private readonly IConfiguration _configuration = configuration;

        public async Task<CommonResponse> Login(LoginRequestDto loginRequest)
        {
            // Getting User based on email from Database
            ApplicationUser? user = await _authRepository.GetUserByEmail(loginRequest.Email);

            if (user != null)
            {
                // verifying entered password with actual password
                bool checkPassword = await _authRepository.CheckPassword(user, loginRequest.Password);
                if (checkPassword)
                {
                    List<string> roles = await _authRepository.GetRolesByUser(user);

                    // Jwt Token Generation
                    string token = _tokenRepository.CreateJwtToken(user, roles);

                    if (string.IsNullOrEmpty(token))
                    {
                        _response.Message = "Token is Empty";
                    }
                    else
                    {
                        LoginResponseDto response = new()
                        {
                            UserId = user.Id,
                            Email = loginRequest.Email,
                            Roles = roles,
                            Token = token,
                            Address = user.Address,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            PhoneNumber = user.PhoneNumber
                        };

                        _response.IsSuccess = true;
                        _response.Data = response;
                        _response.Message = "Login Successful";
                    }
                }
                // If Password Does not Match
                else
                {
                    _response.Message = "Invalid Login Credentials";
                }
            }
            // If User Does not Exists by that email
            else
            {
                _response.Message = "User Does Not Exists. Please Register";
            }
            return _response;
        }

        public async Task<CommonResponse> Register(RegisterRequestDto registerRequest)
        {
            // Checking whether user already exists or not
            ApplicationUser? existingUser = await _authRepository.GetUserByEmail(registerRequest.Email);
            if (existingUser != null)
            {
                _response.Message = "User with this email already exist. Please Login";
            }
            else
            {
                // Create new User
                ApplicationUser user = new()
                {
                    FirstName = registerRequest.FirstName,
                    LastName = registerRequest.LastName,
                    UserName = registerRequest.Email?.Trim(),
                    Email = registerRequest.Email?.Trim(),
                    PhoneNumber = registerRequest.PhoneNumber.Trim(),
                    Address = registerRequest.Address
                };

                IdentityResult userCreationResult = await _authRepository.CreateUser(user, registerRequest.Password);
                if (userCreationResult.Succeeded)
                {
                    // Assign role to new user
                    IdentityResult roleAssignmentResult = await _authRepository.AssignRole(user, registerRequest.Role);
                    if (roleAssignmentResult.Succeeded)
                    {
                        _response.IsSuccess = true;
                        _response.Message = "User Registration Successful";
                        _response.Data = user;
                        SendWelcomeEmail(user);
                    }
                    // If Assigning Role to user fails
                    else
                    {
                        if (roleAssignmentResult.Errors.Any())
                        {
                            foreach (var error in roleAssignmentResult.Errors)
                            {
                                _response.Message = error.Description;
                            }
                        }
                    }
                }
                else
                {
                    // If User Created Fails
                    if (userCreationResult.Errors.Any())
                    {
                        foreach (var error in userCreationResult.Errors)
                        {
                            _response.Message = error.Description;
                        }
                    }
                }
            }
            return _response;
        }

        public async Task<CommonResponse> ChangePassword(ChangePasswordRequestDto changePasswordRequest)
        {
            if (string.IsNullOrEmpty(changePasswordRequest.Email)
                || string.IsNullOrEmpty(changePasswordRequest.NewPassword)
                || string.IsNullOrEmpty(changePasswordRequest.OldPassword))
            {
                _response.Message = "Some Required fields are Empty.";
            }
            else
            {
                // Getting User based on email from Database
                ApplicationUser? user = await _authRepository.GetUserByEmail(changePasswordRequest.Email);

                if (user == null)
                {
                    _response.Message = "User Does not Exists.";
                }
                else if (changePasswordRequest.NewPassword == changePasswordRequest.OldPassword)
                {
                    _response.Message = "Old and New Password can not be same.";
                }
                else
                {
                    // Verfying current password
                    bool checkPassword = await _authRepository.CheckPassword(user, changePasswordRequest.OldPassword);
                    if (checkPassword)
                    {
                        IdentityResult passwordChangeResult = await _authRepository.ChangePassword(user, changePasswordRequest.NewPassword);
                        if (passwordChangeResult.Succeeded)
                        {
                            _response.IsSuccess = true;
                            _response.Message = "Password Changed Successfully.";
                        }
                        else
                        {
                            if (passwordChangeResult.Errors.Any())
                            {
                                foreach (var error in passwordChangeResult.Errors)
                                {
                                    _response.Message = error.Description;
                                }
                            }
                        }
                    }
                    // If Old Password is not correct
                    else
                    {
                        _response.Message = "Old Password is not correct.";
                    }
                }
            }
            return _response;
        }

        public async Task<CommonResponse> ForgotPassword(string email)
        {
            if ((string.IsNullOrEmpty(email)))
            {
                _response.Message = "Email is m=null or Empty";
            }
            else
            {
                ApplicationUser? user = await _authRepository.GetUserByEmail(email);
                if(user != null)
                {
                    string newPassword = GeneratePassword();

                    IdentityResult passwordChangeResult = await _authRepository.ChangePassword(user, newPassword);
                    if (passwordChangeResult.Succeeded)
                    {
                        _response.IsSuccess = true;
                        _response.Message = "Password Changed Successfully.";
                    }
                    else
                    {
                        if (passwordChangeResult.Errors.Any())
                        {
                            foreach (var error in passwordChangeResult.Errors)
                            {
                                _response.Message = error.Description;
                            }
                        }
                    }
                    SendForgotPasswordEmail(user.FirstName, email, newPassword);
                    _response.IsSuccess = true;
                    _response.Message = "A system Generated Password is sent to your email address.";
                }
                else
                {
                    _response.Message = "User with this email Id does not exist.";
                }
            }
            return _response;
        }

        private static string GeneratePassword()
        {
            string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
            string Digits = "0123456789";
            string SpecialCharacters = "!@#$%*";
            var random = new Random();
            int length = random.Next(6,12);

            // Ensure at least one of each required character type
            var password = new StringBuilder();
            password.Append(UppercaseLetters[random.Next(UppercaseLetters.Length)]);
            password.Append(LowercaseLetters[random.Next(LowercaseLetters.Length)]);
            password.Append(Digits[random.Next(Digits.Length)]);
            password.Append(SpecialCharacters[random.Next(SpecialCharacters.Length)]);

            // Fill the rest of the password length with random characters from all sets
            var allCharacters = UppercaseLetters + LowercaseLetters + Digits + SpecialCharacters;
            for (int i = password.Length; i < length; i++)
            {
                password.Append(allCharacters[random.Next(allCharacters.Length)]);
            }

            // Shuffle the password to ensure random distribution
            return new string(password.ToString().OrderBy(c => random.Next()).ToArray());
        }

        private void SendWelcomeEmail(ApplicationUser user)
        {
            string fromMail = _configuration["SMTP:FromEmail"] ?? "";
            string toMail = user.Email ?? "";
            string fromPassword = _configuration["SMTP:Password"] ?? "";

            // Read the HTML template from file
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\4.Infrastructure\\EmailTemplates\\WelcomeUserEmailTemplate.html");
            string body = File.ReadAllText(templatePath);

            body = body
                .Replace("{FirstName}", user.FirstName)
                .Replace("{LastName}", user.LastName)
                .Replace("{Email}", user.Email)
                .Replace("{PhoneNumber}", user.PhoneNumber)
                .Replace("{Address}", user.Address)
                .Replace("{LoginUrl}", _configuration["EComUrl:DevelopmentUrl"] ?? "");


            MailMessage message = new()
            {
                From = new MailAddress(fromMail),
                Subject = "Welcome To The Ecom",
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(toMail));

            SmtpClient smtpClient = new("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true
            };

            smtpClient.Send(message);
        }

        private void SendForgotPasswordEmail(string name, string email, string password)
        {
            string fromMail = _configuration["SMTP:FromEmail"] ?? "";
            string toMail = email ?? "";
            string fromPassword = _configuration["SMTP:Password"] ?? "";

            // Read the HTML template from file
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\4.Infrastructure\\EmailTemplates\\ForgotPasswordEmailTemplate.html");
            string body = File.ReadAllText(templatePath);

            body = body
                .Replace("{Username}", name)
                .Replace("{Email}", email)
                .Replace("{NewPassword}", password);


            MailMessage message = new()
            {
                From = new MailAddress(fromMail),
                Subject = "Forgot Password Request",
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(toMail));

            SmtpClient smtpClient = new("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true
            };

            smtpClient.Send(message);
        }
    }
}
