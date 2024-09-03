using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService._2.BusinessLogic.DTO;
using ProductService._2.BusinessLogic.IBusinessLogic;
using ProductService._4.Infrastructure.Models;

namespace ProductService._1.WebAPI.Controllers
{
    [Route("api/product/stocklevel")]
    [ApiController]
    public class StockLevelController(IStockLevelLogic stockLevelLogic) : ControllerBase
    {
        private readonly IStockLevelLogic _stockLevelLogic = stockLevelLogic;

        [HttpPost]
        [Route("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<CommonResponse> GetAllStockLevels([FromBody] FilterModel filter)
        {
            return await _stockLevelLogic.GetAllStockLevels(filter);
        }

        [HttpGet]
        [Route("Get/{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<CommonResponse> GetStockLevel([FromRoute] Guid id)
        {
            return await _stockLevelLogic.GetStockLevel(id);
        }

        [HttpPost]
        [Route("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<CommonResponse> CreateStockLevel([FromBody] StockLevelRequestDTO stockLevelRequest)
        {
            return await _stockLevelLogic.CreateStockLevel(stockLevelRequest);
        }

        [HttpPut]
        [Route("Update/{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<CommonResponse> UpdateStockLevel([FromBody] StockLevelRequestDTO stockLevelRequest, [FromRoute] Guid id)
        {
            return await _stockLevelLogic.UpdateStockLevel(stockLevelRequest, id);
        }

        [HttpDelete]
        [Route("Delete/{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<CommonResponse> DeleteStockLevel([FromRoute] Guid id)
        {
            return await _stockLevelLogic.DeleteStockLevel(id);
        }

        [HttpGet]
        [Route("GetStockCount/{productId:Guid}")]
        [Authorize]
        public async Task<CommonResponse> GetStockCount([FromRoute]Guid productId)
        {
            return await _stockLevelLogic.GetStockCount(productId);
        }

        [HttpPut]
        [Route("UpdateStockCount")]
        [Authorize]
        public async Task<CommonResponse> UpdateStockCount([FromBody] UpdateStockProductsRequestDTO request)
        {
            return await _stockLevelLogic.UpdateStockCount(request.ProductIds, request.Counts);
        }
    }
}
