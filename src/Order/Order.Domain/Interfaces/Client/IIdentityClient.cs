using Order.Domain.Models;

namespace Order.Domain.Interfaces.Client
{
    public interface IIdentityClient
    {
        Task<User> GetUserByUserId(string userId);
    }
}
