using ProductService._2.BusinessLogic.DTO;
using ProductService._3.DataAccess.Domains;

namespace ProductService._3.DataAccess.IRepositories
{
    public interface IStockLevelRepository
    {
        Task<StockLevel> CreateStockLevel(StockLevel stockLevel);
        Task<StockLevel> DeleteStockLevel(StockLevel stockLevel);
        Task<StockLevel?> GetStockLevelById(Guid Id);
        Task<StockLevel?> GetStockLevelByProductId(Guid Id);
        Task<List<StockLevel>> GetStockLevelsAsync(FilterModel filter);
        Task<StockLevel> UpdateStockLevel(StockLevel stockLevel, StockLevel updatedStockLevel);
    }
}
