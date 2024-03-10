using Basket.Domain.Models;

namespace Basket.Domain.Interfaces.Repositories
{
    public interface IBasketRepository
    {
        Task<Models.Basket> GetByIdAsync(int id);
        Task<Models.Basket> GetByUserIdAsync(string userId);
        Task AddAsync(Models.Basket basket);
        void Update(Models.Basket basket);
        Task SaveChangesAsync();
    }
}
