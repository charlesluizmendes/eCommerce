using Identity.Domain.Interfaces.Repositories;
using Identity.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infraestructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }      

        public async Task<User> GetByIdAsync(string id)
        {
            try
            {
                return await _userManager.FindByIdAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task InsertAsync(User user)
        {
            try
            {
                await _userManager.CreateAsync(user, user.PasswordHash);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateAsync(User user)
        {
            try
            {
                await _userManager.UpdateAsync(user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(User user)
        {
            try
            {
                await _userManager.DeleteAsync(user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _userManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
