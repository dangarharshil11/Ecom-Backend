using CustomerService._2.BusinessLogic.DTO;
using CustomerService._4.Infrastructure.Models;

namespace CustomerService._2.BusinessLogic.IBusinessLogic
{
    public interface IOrderLogic
    {
        Task<CommonResponse> CreateOrder(List<CartResponseDTO> cartResponses);
        Task<CommonResponse> CreateStripeSession(StripeRequestDTO stripeRequest);
        Task<CommonResponse> GetOrderById(Guid orderId);
        Task<CommonResponse> GetOrderByUserId(Guid userId);
        Task<CommonResponse> GetOrders(FilterModel filter);
        Task<CommonResponse> UpdateOrder(UpdateOrderRequestDTO request);
        Task<CommonResponse> ValidateStripeSession(Guid orderId);
    }
}
