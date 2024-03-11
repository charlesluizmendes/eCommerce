using Identity.Domain.Models;

namespace Identity.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(string id);
        Task<User> GetUserByEmailAsync(string email);
        Task InsertAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
    }
}
