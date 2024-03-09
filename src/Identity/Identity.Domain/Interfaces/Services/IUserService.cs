using Identity.Domain.Models;

namespace Identity.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(string id);
        Task InsertAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
    }
}
