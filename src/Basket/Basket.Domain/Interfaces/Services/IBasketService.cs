using Basket.Domain.Models;

namespace Basket.Domain.Interfaces.Services
{
    public interface IBasketService
    {
        Task<Models.Basket> GetAsync();
        Task RemoveAsync(int id);
    }
}
