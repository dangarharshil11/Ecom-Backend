using AutoMapper;
using ProductService._2.BusinessLogic.DTO;
using ProductService._2.BusinessLogic.IBusinessLogic;
using ProductService._3.DataAccess.Domains;
using ProductService._3.DataAccess.IRepositories;
using ProductService._3.DataAccess.Repositories;
using ProductService._4.Infrastructure.Models;

namespace ProductService._2.BusinessLogic.BusinessLogic
{
    public class CategoryLogic(ICategoryRepository categoryRepository, IProductRepository productRepository, IMapper mapper) : ICategoryLogic
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly CommonResponse response = new();
        private readonly IMapper _mapper = mapper;

        public async Task<CommonResponse> CreateCategory(CategoryRequestDTO categoryRequestDTO)
        {
            if (categoryRequestDTO == null ||
                string.IsNullOrEmpty(categoryRequestDTO.CategoryName) ||
                string.IsNullOrEmpty(categoryRequestDTO.CategoryDescription))
            {
                response.Message = "One or More Fields of CategoryRequest is Null or Empty.";
            }
            else
            {
                Category? existingCategory = await _categoryRepository.GetCategoryByName(categoryRequestDTO.CategoryName);

                if (existingCategory == null)
                {
                    Category category = _mapper.Map<Category>(categoryRequestDTO);

                    category = await _categoryRepository.CreateCategory(category);

                    response.Data = category;
                    response.Message = "Category is Created Successfully";
                    response.IsSuccess = true;
                }
                else
                {
                    response.Message = "Category with this Name already exists!!";
                    response.Data = existingCategory;
                }
            }

            return response;
        }

        public async Task<CommonResponse> GetAllCategories(FilterModel filter)
        {
            if(filter.Page == null && filter.PageSize == null)
            {
                filter.Page = 1;
                filter.PageSize = 5;
            }

            List<Category> categories = await _categoryRepository.GetCategoriesAsync(filter);

            if (categories.Count == 0)
            {
                response.Message = "No Catgories Found";
            }
            response.IsSuccess = true;
            response.Data = categories;

            return response;
        }

        public async Task<CommonResponse> GetCategoryList()
        {
            response.Data =  await _categoryRepository.GetCategoryList();
            response.IsSuccess = true;

            return response;
        }

        public async Task<CommonResponse> GetCategory(string? categoryName, Guid? Id)
        {
            Category? category;
            if (!string.IsNullOrEmpty(categoryName))
            {
                category = await _categoryRepository.GetCategoryByName(categoryName);
                if (category != null)
                {
                    response.IsSuccess = true;
                    response.Data = category;
                }
                else
                {
                    response.Message = "Category with this name does not exists.";
                }
            }
            else if (Guid.Empty != Id && Id != null)
            {
                category = await _categoryRepository.GetCategoryById((Guid)Id);
                if (category != null)
                {
                    response.IsSuccess = true;
                    response.Data = category;
                }
                else
                {
                    response.Message = "Category with this Id does not exists.";
                }
            }
            else
            {
                response.Message = "CategoryName or Id is required.";
            }
            return response;
        }

        public async Task<CommonResponse> DeleteCategory(Guid Id)
        {
            if(Guid.Empty != Id)
            {
                Category? category = await _categoryRepository.GetCategoryById(Id);

                List<Product> products = await _productRepository.GetProductsByCategory(Id);

                if (products.Count > 0)
                {
                    response.Message = "Some Product Belong to this Category Remove them before Deleting Category.";
                }
                else
                {
                    if (category != null)
                    {
                        category = await _categoryRepository.DeleteCategory(category);

                        response.IsSuccess = true;
                        response.Data = category;
                        response.Message = "Category Deleted Successfully.";
                    }
                    else
                    {
                        response.Message = "Category with this Id does not exists.";
                    }
                }
            }
            else
            {
                response.Message = "Category Id is Empty";
            }

            return response;
        }

        public async Task<CommonResponse> UpdateCategory(CategoryRequestDTO categoryRequestDTO, Guid categoryId)
        {
            if (categoryRequestDTO == null ||
                string.IsNullOrEmpty(categoryRequestDTO.CategoryName) ||
                string.IsNullOrEmpty(categoryRequestDTO.CategoryDescription))
            {
                response.Message = "One or More Fields of CategoryRequest is Null or Empty.";
            }
            else
            {
                Category? existingCategory = await _categoryRepository.GetCategoryById(categoryId);

                if (existingCategory != null)
                {
                    Category category = _mapper.Map<Category>(categoryRequestDTO);
                    category.CreatedDate = existingCategory.CreatedDate;
                    category.Id = categoryId;

                    category = await _categoryRepository.UpdateCategory(existingCategory, category);

                    response.IsSuccess = true;
                    response.Data = category;
                    response.Message = "Category Updated Successfully.";
                }
                else
                {
                    response.Message = "Category with this Id does not exists.";
                }
            }

            return response;
        }
    }
}
