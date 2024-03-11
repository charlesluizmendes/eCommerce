using Identity.Domain.Models;

namespace Identity.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(string id);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> InsertAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(User user);
    }
}
