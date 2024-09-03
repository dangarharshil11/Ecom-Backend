using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService._2.BusinessLogic.DTO;
using ProductService._2.BusinessLogic.IBusinessLogic;
using ProductService._4.Infrastructure.Models;

namespace ProductService._1.WebAPI.Controllers
{
    [Route("api/product/product")]
    [ApiController]
    public class ProductController(IProductLogic productLogic) : ControllerBase
    {
        private readonly IProductLogic _productLogic = productLogic;

        [HttpPost]
        [Route("GetAll")]
        public async Task<CommonResponse> GetAllProducts([FromBody] FilterModel filter)
        {
            return await _productLogic.GetAllProducts(filter);
        }

        [HttpGet]
        [Route("Get/{name}")]
        public async Task<CommonResponse> GetProductByName([FromRoute] string name)
        {
            return await _productLogic.GetProduct(name, null);
        }

        [HttpGet]
        [Route("Get/{id:Guid}")]
        public async Task<CommonResponse> GetProductById([FromRoute] Guid id)
        {
            return await _productLogic.GetProduct(null, id);
        }

        [HttpGet]
        [Route("GetProductList")]
        public async Task<CommonResponse> GetProductList()
        {
            return await _productLogic.GetProductList();
        }

        [HttpPost]
        [Route("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<CommonResponse> CreateProduct([FromBody] ProductRequestDTO productRequest)
        {
            return await _productLogic.CreateProduct(productRequest);
        }

        [HttpPut]
        [Route("Update/{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<CommonResponse> UpdateProduct([FromBody] ProductRequestDTO productRequest, [FromRoute] Guid id)
        {
            return await _productLogic.UpdateProduct(productRequest, id);
        }

        [HttpDelete]
        [Route("Delete/{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<CommonResponse> DeleteProduct([FromRoute] Guid id)
        {
            return await _productLogic.DeleteProduct(id);
        }

        [HttpPost]
        [Route("GetProducts")]
        [Authorize]
        public async Task<CommonResponse> GetProductsByProductIds([FromBody]List<Guid> productIds)
        {
            return await _productLogic.GetProducts(productIds);
        }
    }
}
