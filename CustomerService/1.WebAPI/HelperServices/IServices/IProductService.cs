using CustomerService._2.BusinessLogic.DTO;

namespace CustomerService._1.WebAPI.HelperServices.IServices
{
    public interface IProductService
    {
        Task<string> CheckForStock(Guid productId, int count);
        Task<List<ProductDTO>> GetProducts(List<Guid> productIds);
        Task<int?> GetStock(Guid productId);
        Task<bool> UpdateStock(List<Guid> productIds, List<int> productCounts);
    }
}
