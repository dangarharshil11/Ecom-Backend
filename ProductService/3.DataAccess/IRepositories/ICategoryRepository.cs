using ProductService._2.BusinessLogic.DTO;
using ProductService._3.DataAccess.Domains;

namespace ProductService._3.DataAccess.IRepositories
{
    public interface ICategoryRepository
    {
        Task<Category> CreateCategory(Category category);
        Task<Category> DeleteCategory(Category category);
        Task<List<Category>> GetCategoriesAsync(FilterModel filter);
        Task<Category?> GetCategoryById(Guid Id);
        Task<Category?> GetCategoryByName(string Name);
        Task<Dictionary<Guid, string>> GetCategoryList();
        Task<Category> UpdateCategory(Category category, Category updatedCategory);
    }
}
