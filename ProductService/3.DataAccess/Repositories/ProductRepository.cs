using Microsoft.EntityFrameworkCore;
using ProductService._2.BusinessLogic.DTO;
using ProductService._3.DataAccess.Context;
using ProductService._3.DataAccess.Domains;
using ProductService._3.DataAccess.IRepositories;
using System.Linq.Expressions;

namespace ProductService._3.DataAccess.Repositories
{
    public class ProductRepository(ProductDbContext dbContext) : IProductRepository
    {
        private readonly ProductDbContext _dbContext = dbContext;
        public static int totalRecords { get; set; } = 0;

        public async Task<Product> CreateProduct(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        public async Task<Product?> GetProductById(Guid Id)
        {
            Product? product = await _dbContext.Products.Include(x => x.Category).FirstOrDefaultAsync(c => c.Id == Id);

            if(product != null) { 
                product.StockItems = (await _dbContext.StockLevels.FirstOrDefaultAsync(c => c.ProductId == product.Id))?.CurrentStockItems;
            }
            return product;
        }

        public async Task<Product?> GetProductByName(string Name)
        {
            Product? product = await _dbContext.Products.Include(x => x.Category).FirstOrDefaultAsync(c => c.ProductName.ToLower() == Name.ToLower());
            if (product != null)
            {
                product.StockItems = (await _dbContext.StockLevels.FirstOrDefaultAsync(c => c.ProductId == product.Id))?.CurrentStockItems;
            }
            return product;
        }

        public async Task<Product> UpdateProduct(Product Product, Product updatedProduct)
        {
            _dbContext.Entry(Product).CurrentValues.SetValues(updatedProduct);
            await _dbContext.SaveChangesAsync();
            return updatedProduct;
        }

        public async Task<Product> DeleteProduct(Product product)
        {
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<List<Product>> GetProductsAsync(FilterModel filter)
        {
            IQueryable<Product> query = _dbContext.Products.Include(x => x.Category);

            totalRecords = query.Count();

            // Apply filtering
            if (!string.IsNullOrEmpty(filter.SearchText) && !string.IsNullOrEmpty(filter.SearchColumn))
            {
                query = query.Where(p => EF.Property<string>(p, filter.SearchColumn).Contains(filter.SearchText));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(filter.SortByColumn))
            {
                var parameter = Expression.Parameter(typeof(Product), "p");
                var property = Expression.Property(parameter, filter.SortByColumn);
                var sortExpression = Expression.Lambda<Func<Product, object>>(Expression.Convert(property, typeof(object)), parameter);

                query = filter.SortOrder?.ToLower() == "descending"
                    ? query.OrderByDescending(sortExpression)
                    : query.OrderBy(sortExpression);
            }

            // Apply pagination
            var skip = ((filter.Page ?? 1) - 1) * filter.PageSize ?? 5;
            var pagedResult = await query.Skip(skip).Take(filter.PageSize ?? 5).ToListAsync();

            foreach(var item in pagedResult)
            {
                item.StockItems = (await _dbContext.StockLevels.FirstOrDefaultAsync(c => c.ProductId == item.Id))?.CurrentStockItems;
            }

            return pagedResult;
        }

        public async Task<List<Product>> GetProductsByCategory(Guid categoryId)
        {
            var Result = await _dbContext.Products.Where(x => x.CategoryId == categoryId).ToListAsync();
            foreach (var item in Result)
            {
                item.StockItems = (await _dbContext.StockLevels.FirstOrDefaultAsync(c => c.ProductId == item.Id))?.CurrentStockItems;
            }
            return Result;
        }

        public async Task<Dictionary<Guid, string>> GetProductList()
        {
            return await _dbContext.Products.Include(x => x.Category).ToDictionaryAsync(c => c.Id, c => c.ProductName);

        }

        public async Task<List<Product>> GetProducts(List<Guid> productIds)
        {
            return await _dbContext.Products.Include(x => x.Category).Where(x => productIds.Contains(x.Id)).ToListAsync();
        }
    }
}
