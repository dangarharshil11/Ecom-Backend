using ProductService._2.BusinessLogic.DTO;
using ProductService._4.Infrastructure.Models;

namespace ProductService._2.BusinessLogic.IBusinessLogic
{
    public interface ICategoryLogic
    {
        Task<CommonResponse> CreateCategory(CategoryRequestDTO categoryRequestDTO);
        Task<CommonResponse> DeleteCategory(Guid Id);
        Task<CommonResponse> GetAllCategories(FilterModel filter);
        Task<CommonResponse> GetCategory(string? categoryName, Guid? Id);
        Task<CommonResponse> GetCategoryList();
        Task<CommonResponse> UpdateCategory(CategoryRequestDTO categoryRequestDTO, Guid categoryId);
    }
}
