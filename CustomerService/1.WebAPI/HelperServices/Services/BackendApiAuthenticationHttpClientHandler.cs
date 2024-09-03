using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace CustomerService._1.WebAPI.HelperServices
{
    public class BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor accessor) : DelegatingHandler
    {
        private readonly IHttpContextAccessor _accessor = accessor;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _accessor.HttpContext.GetTokenAsync("access_token");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
