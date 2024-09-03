using Microsoft.EntityFrameworkCore;
using ProductService._2.BusinessLogic.DTO;
using ProductService._3.DataAccess.Context;
using ProductService._3.DataAccess.Domains;
using ProductService._3.DataAccess.IRepositories;
using System.Linq.Expressions;

namespace ProductService._3.DataAccess.Repositories
{
    public class StockLevelRepository(ProductDbContext dbContext) : IStockLevelRepository
    {
        private readonly ProductDbContext _dbContext = dbContext;
        public static int totalRecords { get; set; } = 0;

        public async Task<StockLevel> CreateStockLevel(StockLevel stockLevel)
        {
            await _dbContext.StockLevels.AddAsync(stockLevel);
            await _dbContext.SaveChangesAsync();

            return stockLevel;
        }

        public async Task<StockLevel?> GetStockLevelById(Guid Id)
        {
            return await _dbContext.StockLevels.Include(x => x.Product).FirstOrDefaultAsync(c => c.Id == Id);
        }

        public async Task<StockLevel?> GetStockLevelByProductId(Guid Id)
        {
            return await _dbContext.StockLevels.Include(x => x.Product).FirstOrDefaultAsync(c => c.ProductId == Id);
        }

        public async Task<StockLevel> UpdateStockLevel(StockLevel stockLevel, StockLevel updatedStockLevel)
        {
            _dbContext.Entry(stockLevel).CurrentValues.SetValues(updatedStockLevel);
            await _dbContext.SaveChangesAsync();
            return updatedStockLevel;
        }

        public async Task<StockLevel> DeleteStockLevel(StockLevel stockLevel)
        {
            _dbContext.StockLevels.Remove(stockLevel);
            await _dbContext.SaveChangesAsync();
            return stockLevel;
        }

        public async Task<List<StockLevel>> GetStockLevelsAsync(FilterModel filter)
        {
            IQueryable<StockLevel> query = _dbContext.StockLevels.Include(x => x.Product);
            totalRecords = query.Count();

            // Apply filtering
            if (!string.IsNullOrEmpty(filter.SearchText) && !string.IsNullOrEmpty(filter.SearchColumn))
            {
                query = query.Where(p => EF.Property<string>(p, filter.SearchColumn).Contains(filter.SearchText));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(filter.SortByColumn))
            {
                var parameter = Expression.Parameter(typeof(StockLevel), "p");
                var property = Expression.Property(parameter, filter.SortByColumn);
                var sortExpression = Expression.Lambda<Func<StockLevel, object>>(Expression.Convert(property, typeof(object)), parameter);

                query = filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(sortExpression)
                    : query.OrderBy(sortExpression);
            }

            // Apply pagination
            var skip = ((filter.Page ?? 1) - 1) * filter.PageSize ?? 5;
            var pagedResult = await query.Skip(skip).Take(filter.PageSize ?? 5).ToListAsync();

            return pagedResult;
        }
    }
}
