using Identity.Domain.Models;

namespace Identity.Domain.Interfaces.Identity
{
    public interface IUserIdentity
    {
        Task<AccessToken> CreateTokenByUserIdAsync(string userId);
        string GetUserIdFromToken();
    }
}
