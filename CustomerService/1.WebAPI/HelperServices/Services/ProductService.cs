using CustomerService._1.WebAPI.HelperServices.IServices;
using CustomerService._2.BusinessLogic.DTO;
using CustomerService._4.Infrastructure.Models;
using Newtonsoft.Json;
using System.Text;

namespace CustomerService._1.WebAPI.HelperServices.Services
{
    public class ProductService(IHttpClientFactory httpClientFactory) : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<List<ProductDTO>> GetProducts(List<Guid> productIds)
        {
            var client = _httpClientFactory.CreateClient("Product");
            var data = new StringContent(JsonConvert.SerializeObject(productIds), Encoding.UTF8, "application/json");
            var apiResponse = await client.PostAsync($"/api/product/product/GetProducts", data);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            CommonResponse Response = JsonConvert.DeserializeObject<CommonResponse>(Convert.ToString(apiContent)) ?? new CommonResponse() { };

            if (Response.IsSuccess && Response.Data != null)
            {
                return JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(Response.Data) ?? "") ?? [];
            }
            return [];
        }

        public async Task<string> CheckForStock(Guid productId, int count)
        {
            int? existingCount = await GetStock(productId);

            if (existingCount.HasValue)
            {
                if (existingCount.Value == 0)
                {
                    return "Product is Out Of Stock.";
                }
                else if (existingCount < count)
                {
                    return $"Only {existingCount} are available in the stock.";
                }
                else
                {
                    return "";
                }
            }
            return "Product is not in the Stock";
        }

        public async Task<int?> GetStock(Guid productId)
        {
            var client = _httpClientFactory.CreateClient("Product");
            var apiResponse = await client.GetAsync($"api/product/stocklevel/GetStockCount/{productId}");
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            CommonResponse Response = JsonConvert.DeserializeObject<CommonResponse>(Convert.ToString(apiContent)) ?? new CommonResponse() { };

            if (Response.IsSuccess && Response.Data != null)
            {
                return JsonConvert.DeserializeObject<int>(Convert.ToString(Response.Data) ?? "");
            }
            return null;
        }
        
        public async Task<bool> UpdateStock(List<Guid> productIds, List<int> productCounts)
        {
            var client = _httpClientFactory.CreateClient("Product");
            UpdateStockProductsRequest request = new()
            {
                ProductIds = productIds,
                Counts = productCounts
            };
            var data = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var apiResponse = await client.PutAsync($"api/product/stocklevel/UpdateStockCount", data);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            CommonResponse Response = JsonConvert.DeserializeObject<CommonResponse>(Convert.ToString(apiContent)) ?? new CommonResponse() { };

            return Response.IsSuccess;
        }
    }
}
