using CustomerService._2.BusinessLogic.DTO;
using CustomerService._4.Infrastructure.Models;

namespace CustomerService._2.BusinessLogic.IBusinessLogic
{
    public interface ICartLogic
    {
        Task<CommonResponse> DeleteById(Guid Id);
        Task<CommonResponse> EmptyCart(Guid userId);
        Task<CommonResponse> GetByUserId(Guid userId);
        Task<CommonResponse> UpsertCart(CartRequestDTO cartRequestDTO);
    }
}
