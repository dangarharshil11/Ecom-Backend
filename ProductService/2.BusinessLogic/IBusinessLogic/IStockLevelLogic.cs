using ProductService._2.BusinessLogic.DTO;
using ProductService._4.Infrastructure.Models;

namespace ProductService._2.BusinessLogic.IBusinessLogic
{
    public interface IStockLevelLogic
    {
        Task<CommonResponse> CreateStockLevel(StockLevelRequestDTO stockLevelRequestDTO);
        Task<CommonResponse> DeleteStockLevel(Guid Id);
        Task<CommonResponse> GetAllStockLevels(FilterModel filter);
        Task<CommonResponse> GetStockCount(Guid productId);
        Task<CommonResponse> GetStockLevel(Guid Id);
        Task<CommonResponse> UpdateStockCount(List<Guid> productIds, List<int> productCounts);
        Task<CommonResponse> UpdateStockLevel(StockLevelRequestDTO stockLevelRequestDTO, Guid stockLevelId);
    }
}
