using Catalog.Domain.Interfaces.Repositories;
using Catalog.Domain.Interfaces.Services;
using Catalog.Domain.Models;

namespace Catalog.Domain.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Product>> GetListAsync()
        {
            return await _repository.GetListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }
}
