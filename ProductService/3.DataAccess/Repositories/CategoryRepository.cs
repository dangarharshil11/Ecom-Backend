using Microsoft.EntityFrameworkCore;
using ProductService._2.BusinessLogic.DTO;
using ProductService._3.DataAccess.Context;
using ProductService._3.DataAccess.Domains;
using ProductService._3.DataAccess.IRepositories;
using System.Linq.Expressions;

namespace ProductService._3.DataAccess.Repositories
{
    public class CategoryRepository(ProductDbContext dbContext) : ICategoryRepository
    {
        private readonly ProductDbContext _dbContext = dbContext;

        public async Task<Category> CreateCategory(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            return category;
        }

        public async Task<Category?> GetCategoryById(Guid Id)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == Id);
        }

        public async Task<Category?> GetCategoryByName(string Name)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName.ToLower() == Name.ToLower());
        }

        public async Task<Category> UpdateCategory(Category category, Category updatedCategory)
        {
            _dbContext.Entry(category).CurrentValues.SetValues(updatedCategory);
            await _dbContext.SaveChangesAsync();
            return updatedCategory;
        }

        public async Task<Category> DeleteCategory(Category category)
        {
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<List<Category>> GetCategoriesAsync(FilterModel filter)
        {
            IQueryable<Category> query = _dbContext.Categories;

            // Apply filtering
            if (!string.IsNullOrEmpty(filter.SearchText) && !string.IsNullOrEmpty(filter.SearchColumn))
            {
                query = query.Where(p => EF.Property<string>(p, filter.SearchColumn).Contains(filter.SearchText));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(filter.SortByColumn))
            {
                var parameter = Expression.Parameter(typeof(Category), "p");
                var property = Expression.Property(parameter, filter.SortByColumn);
                var sortExpression = Expression.Lambda<Func<Category, object>>(Expression.Convert(property, typeof(object)), parameter);

                query = filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(sortExpression)
                    : query.OrderBy(sortExpression);
            }

            // Apply pagination
            var skip = ((filter.Page ?? 1) - 1) * filter.PageSize ?? 5;
            var pagedResult = await query.Skip(skip).Take(filter.PageSize ?? 5).ToListAsync();

            return pagedResult;
        }

        public async Task<Dictionary<Guid, string>> GetCategoryList()
        {
            return await _dbContext.Categories.ToDictionaryAsync(c => c.Id, c => c.CategoryName);

        }
    }
}
