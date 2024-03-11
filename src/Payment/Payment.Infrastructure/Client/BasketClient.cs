using Newtonsoft.Json;
using Payment.Domain.Interfaces.Client;

namespace Payment.Infrastructure.Client
{
    public class BasketClient : IBasketClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BasketClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }        

        public async Task<Domain.Models.Basket> GetBaskeAsync()
        {
            var client = _httpClientFactory.CreateClient("Basket");

            try
            {
                var response = await client.GetAsync($"api/Basket/Get");
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return null;

                return JsonConvert.DeserializeObject<Domain.Models.Basket>(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveBasketByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("Basket");

            try
            {
                await client.DeleteAsync($"api/Basket/Remove/{id}");                
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
