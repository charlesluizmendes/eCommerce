using Microsoft.AspNetCore.Http;

namespace Basket.Application.Handlers
{
    public class CatalogHttpClientHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CatalogHttpClientHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            
            request.Headers.Add("Authorization", token);

            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return null;

                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
