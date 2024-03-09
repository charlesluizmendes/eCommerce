using Payment.Domain.Models;

namespace Payment.Domain.Interfaces.Client
{
    public interface IBasketClient
    {
        Task<Basket> GetBasketByUserIdAsync(string userId);
        Task DeleteBasketByUserIdAsync(string userId);
    }
}
