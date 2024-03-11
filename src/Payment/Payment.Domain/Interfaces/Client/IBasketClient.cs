using Payment.Domain.Models;

namespace Payment.Domain.Interfaces.Client
{
    public interface IBasketClient
    {
        Task<Basket> GetBaskeAsync();
        Task RemoveBasketByIdAsync(int id);
    }
}
