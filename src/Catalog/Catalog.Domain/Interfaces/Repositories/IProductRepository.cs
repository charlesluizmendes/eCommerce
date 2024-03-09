using Catalog.Domain.Models;

namespace Catalog.Domain.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetListAsync();
        Task<Product> GetByIdAsync(int id);
    }
}
