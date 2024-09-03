using CustomerService._2.BusinessLogic.DTO;
using CustomerService._2.BusinessLogic.IBusinessLogic;
using CustomerService._4.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService._1.WebAPI.Controllers
{
    [Route("api/customer/cart")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class CartController(ICartLogic cartLogic) : ControllerBase
    {
        private readonly ICartLogic _cartLogic = cartLogic;

        [HttpGet]
        [Route("GetCart/{userId:Guid}")]
        public async Task<CommonResponse> GetCart([FromRoute]Guid userId)
        {
            return await _cartLogic.GetByUserId(userId);
        }

        [HttpPost]
        [Route("UpdateCart")]
        public async Task<CommonResponse> UpdateCart([FromBody]CartRequestDTO cartRequest)
        {
            return await _cartLogic.UpsertCart(cartRequest);
        }

        [HttpDelete]
        [Route("DeleteCartItem/{Id:Guid}")]
        public async Task<CommonResponse> DeleteCartItems([FromRoute]Guid Id)
        {
            return await _cartLogic.DeleteById(Id);
        }

        [HttpGet]
        [Route("EmptyCart/{userId:Guid}")]
        public async Task<CommonResponse> EmptyCart([FromRoute]Guid userId)
        {
            return await _cartLogic.EmptyCart(userId);
        }
    }
}
