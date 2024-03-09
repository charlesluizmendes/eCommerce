using Identity.Domain.Models;

namespace Identity.Domain.Interfaces.Repositories
{
    public interface IAccessTokenRepository
    {
        Task<AccessToken> CreateTokenByEmailAsync(string email);
    }
}
