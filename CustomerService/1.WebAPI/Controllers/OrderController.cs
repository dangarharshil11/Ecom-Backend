using CustomerService._2.BusinessLogic.DTO;
using CustomerService._2.BusinessLogic.IBusinessLogic;
using CustomerService._4.Infrastructure.Constants;
using CustomerService._4.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService._1.WebAPI.Controllers
{
    [Route("api/customer/order")]
    [ApiController]
    public class OrderController(IOrderLogic orderLogic) : ControllerBase
    {
        private readonly IOrderLogic _orderLogic = orderLogic;

        [HttpPost]
        [Route("CreateOrder")]
        [Authorize]
        public async Task<CommonResponse> CreateOrder([FromBody] List<CartResponseDTO> cartResponses)
        {
            return await _orderLogic.CreateOrder(cartResponses);
        }

        [HttpPost]
        [Route("CreatePaymentSession")]
        [Authorize]
        public async Task<CommonResponse> CreatePaymentSession(StripeRequestDTO stripeRequest)
        {
            return await _orderLogic.CreateStripeSession(stripeRequest);
        }

        [HttpGet]
        [Route("ValidatePayment/{orderId:Guid}")]
        [Authorize]
        public async Task<CommonResponse> ValidatePayment([FromRoute] Guid orderId)
        {
            return await _orderLogic.ValidateStripeSession(orderId);    
        }

        [HttpGet]
        [Route("GetOrder/{orderId:Guid}")]
        [Authorize]
        public async Task<CommonResponse> GetOrderById([FromRoute]Guid orderId)
        {
            return await _orderLogic.GetOrderById(orderId);
        }

        [HttpGet]
        [Route("GetOrdersByUserId/{userId:Guid}")]
        [Authorize]
        public async Task<CommonResponse> GetOrdersByUserId([FromRoute]Guid userId)
        {
            return await _orderLogic.GetOrderByUserId(userId);
        }

        [HttpPost]
        [Route("GetOrders")]
        public async Task<CommonResponse> GetOrders([FromBody] FilterModel filter)
        {
            return await _orderLogic.GetOrders(filter);
        }

        [HttpPost]
        [Route("UpdateOrder")]
        [Authorize]
        public async Task<CommonResponse> UpdateOrderS([FromBody] UpdateOrderRequestDTO request)
        {
            if (User.IsInRole("Customer") && request.Status != Constants.Status_Canceled) 
            {
                return new CommonResponse { Message = "Customers can not change order status." };
            }
            return await _orderLogic.UpdateOrder(request);
        }
    }
}
