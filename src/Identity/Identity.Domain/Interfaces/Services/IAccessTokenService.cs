using Identity.Domain.Models;

namespace Identity.Domain.Interfaces.Services
{
    public interface IAccessTokenService
    {
        Task<AccessToken> CreateAcessTokenByUserAsync(User user);
    }
}
