using CustomerService._3.DataAccess.Domains;

namespace CustomerService._3.DataAccess.IRepositories
{
    public interface ICartRepository
    {
        Task<Cart> Create(Cart cart);
        Task<Cart> Delete(Cart cart);
        Task<List<Cart>> GetByUserId(Guid userId);
        Task<Cart?> GetByProductId(Guid productId);
        Task<Cart> Update(Cart cart, Cart updatedCart);
        Task<Cart?> GetById(Guid Id);
    }
}
