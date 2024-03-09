using Basket.Domain.Interfaces.Client;
using Basket.Domain.Models;
using Newtonsoft.Json;

namespace Basket.Infrastructure.Client
{
    public class CatalogClient : ICatalogClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CatalogClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("Catalog");

            try
            {
                var response = await client.GetAsync($"api/Product/{id}");
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return null;

                return JsonConvert.DeserializeObject<Product>(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
