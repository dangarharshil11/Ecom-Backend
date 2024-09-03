using IdentityService._3.DataAccess.Domains;

namespace IdentityService._3.DataAccess.IRepositories
{
    public interface ITokenRepository
    {
        string CreateJwtToken(ApplicationUser user, List<string> roles);
    }
}
