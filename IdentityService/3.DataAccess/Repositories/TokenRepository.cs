using IdentityService._3.DataAccess.Domains;
using IdentityService._3.DataAccess.IRepositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService._3.DataAccess.Repositories
{
    public class TokenRepository(IConfiguration configuration) : ITokenRepository
    {
        private readonly IConfiguration _configuration = configuration;

        public string CreateJwtToken(ApplicationUser user, List<string> roles)
        {
            if (user.Email == null)
            {
                return string.Empty;
            }

            // Adding user email, fullname and userId into claims
            List<Claim> claims = [
                new(ClaimTypes.Email, user.Email),
                new("Id", user.Id)
            ];

            // Adding Roles to the claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "N+RQPbnPa4pL+ldbTeELbuBq8B6OhtnNS68O+HjKfoXyov342ntYkvtyc0aEb+5/"));
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken Token = new(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(300),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
    }
}
