using CustomerService._1.WebAPI.HelperServices.IServices;
using CustomerService._2.BusinessLogic.DTO;
using CustomerService._4.Infrastructure.Models;
using Newtonsoft.Json;

namespace CustomerService._1.WebAPI.HelperServices.Services
{
    public class IdentityService(IHttpClientFactory httpClientFactory) : IIdentityService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        public async Task<UserDTO?> GetUser(Guid userId)
        {
            var client = _httpClientFactory.CreateClient("Identity");
            var apiResponse = await client.GetAsync($"/api/identity/user/getUserById/{userId}");
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            CommonResponse Response = JsonConvert.DeserializeObject<CommonResponse>(Convert.ToString(apiContent)) ?? new CommonResponse() { };

            if (Response.IsSuccess && Response.Data != null)
            {
                return JsonConvert.DeserializeObject<UserDTO>(Convert.ToString(Response.Data) ?? "");
            }
            return null;
        }
    }
}
