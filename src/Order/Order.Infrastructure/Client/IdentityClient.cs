using Newtonsoft.Json;
using Order.Domain.Interfaces.Client;
using Order.Domain.Models;

namespace Order.Infrastructure.Client
{
    public class IdentityClient : IIdentityClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IdentityClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<User> GetUserByUserId(string userId)
        {
            var client = _httpClientFactory.CreateClient("Identity");

            try
            {
                var response = await client.GetAsync($"api/User/{userId}");
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return null;

                return JsonConvert.DeserializeObject<User>(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
