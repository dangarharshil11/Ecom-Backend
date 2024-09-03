using ProductService._2.BusinessLogic.DTO;
using ProductService._4.Infrastructure.Models;

namespace ProductService._2.BusinessLogic.IBusinessLogic
{
    public interface IProductLogic
    {
        Task<CommonResponse> CreateProduct(ProductRequestDTO productRequestDTO);
        Task<CommonResponse> DeleteProduct(Guid Id);
        Task<CommonResponse> GetAllProducts(FilterModel filter);
        Task<CommonResponse> GetProduct(string? productName, Guid? Id);
        Task<CommonResponse> GetProductList();
        Task<CommonResponse> GetProducts(List<Guid> productIds);
        Task<CommonResponse> UpdateProduct(ProductRequestDTO productRequestDTO, Guid productId);
    }
}
