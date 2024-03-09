using Basket.Domain.Models;

namespace Basket.Domain.Interfaces.Services
{
    public interface IBasketService
    {
        Task<Models.Basket> GetByUserIdAsync(string userId);
        Task RemoveAsync(int id);
    }
}
