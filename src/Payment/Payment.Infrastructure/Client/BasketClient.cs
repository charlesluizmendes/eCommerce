using Newtonsoft.Json;
using Payment.Domain.Interfaces.Client;
using Payment.Domain.Models;

namespace Payment.Infrastructure.Client
{
    public class BasketClient : IBasketClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BasketClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }        

        public async Task<Basket> GetBasketByUserIdAsync(string userId)
        {
            var client = _httpClientFactory.CreateClient("Basket");

            try
            {
                var response = await client.GetAsync($"api/Basket/{userId}");
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return null;

                return JsonConvert.DeserializeObject<Basket>(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteBasketByUserIdAsync(string userId)
        {
            var client = _httpClientFactory.CreateClient("Basket");

            try
            {
                await client.DeleteAsync($"api/Basket/{userId}");                
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
