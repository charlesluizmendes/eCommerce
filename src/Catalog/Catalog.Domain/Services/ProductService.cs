using Catalog.Domain.Core;
using Catalog.Domain.Interfaces.Repositories;
using Catalog.Domain.Interfaces.Services;
using Catalog.Domain.Models;

namespace Catalog.Domain.Services
{
    public class ProductService : IProductService
    {
        private readonly NotificationContext _notification;
        private readonly IProductRepository _repository;

        public ProductService(NotificationContext notification,
            IProductRepository repository)
        {
            _notification = notification;   
            _repository = repository;
        }

        public async Task<IEnumerable<Product>> GetListAsync()
        {
            var products = await _repository.GetListAsync();

            if (products.Count() == 0)
                _notification.AddNotification("Não foi encontrado nenhum Produto");

            return products;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                _notification.AddNotification("Não foi encontrado nenhum Produto");

            return product;
        }
    }
}
