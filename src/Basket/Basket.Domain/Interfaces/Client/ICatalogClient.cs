using Basket.Domain.Models;

namespace Basket.Domain.Interfaces.Client
{
    public interface ICatalogClient
    {
        Task<Product> GetProductByIdAsync(int id);
    }
}
