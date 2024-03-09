using Catalog.Domain.Models;

namespace Catalog.Domain.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetListAsync();
        Task<Product> GetByIdAsync(int id);
    }
}
