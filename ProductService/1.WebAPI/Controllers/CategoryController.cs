using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService._2.BusinessLogic.DTO;
using ProductService._2.BusinessLogic.IBusinessLogic;
using ProductService._4.Infrastructure.Models;

namespace ProductService._1.WebAPI.Controllers
{
    [Route("api/product/category")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryController(ICategoryLogic categoryLogic) : ControllerBase
    {
        private readonly ICategoryLogic _categoryLogic = categoryLogic;

        [HttpPost]
        [Route("GetAll")]
        public async Task<CommonResponse> GetAllCategories([FromBody] FilterModel filter)
        {
            return await _categoryLogic.GetAllCategories(filter);
        }

        [HttpGet]
        [Route("GetCategoryList")]
        public async Task<CommonResponse> GetCategoryList()
        {
            return await _categoryLogic.GetCategoryList();
        }

        [HttpGet]
        [Route("Get/{name}")]
        public async Task<CommonResponse> GetCategoryByName([FromRoute] string name)
        {
            return await _categoryLogic.GetCategory(name, null);
        }

        [HttpGet]
        [Route("Get/{id:Guid}")]
        public async Task<CommonResponse> GetCategoryById([FromRoute] Guid id)
        {
            return await _categoryLogic.GetCategory(null, id);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<CommonResponse> CreateCategory([FromBody] CategoryRequestDTO categoryRequest)
        {
            return await _categoryLogic.CreateCategory(categoryRequest);
        }

        [HttpPut]
        [Route("Update/{id:Guid}")]
        public async Task<CommonResponse> UpdateCategory([FromBody] CategoryRequestDTO categoryRequest, [FromRoute] Guid id)
        {
            return await _categoryLogic.UpdateCategory(categoryRequest, id);
        }

        [HttpDelete]
        [Route("Delete/{id:Guid}")]
        public async Task<CommonResponse> DeleteCategory([FromRoute] Guid id)
        {
            return await _categoryLogic.DeleteCategory(id);
        }
    }
}
