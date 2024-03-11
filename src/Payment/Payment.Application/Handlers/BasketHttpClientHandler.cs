using Microsoft.AspNetCore.Http;

namespace Payment.Application.Handlers
{
    public class BasketHttpClientHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BasketHttpClientHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
           
            request.Headers.Add("Authorization", token);

            try
            {
                return await base.SendAsync(request, cancellationToken); 
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
