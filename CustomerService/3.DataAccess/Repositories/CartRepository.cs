using CustomerService._3.DataAccess.Context;
using CustomerService._3.DataAccess.Domains;
using CustomerService._3.DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CustomerService._3.DataAccess.Repositories
{
    public class CartRepository(CustomerDbContext dbContext) : ICartRepository
    {
        private readonly CustomerDbContext _dbContext = dbContext;

        public async Task<Cart?> GetById(Guid Id)
        {
            return await _dbContext.Carts.FirstOrDefaultAsync(c => c.Id == Id);
        }

        public async Task<List<Cart>> GetByUserId(Guid userId)
        {
            List<Cart> carts;

            carts = await _dbContext.Carts.Where(x => x.UserId == userId).ToListAsync();

            return carts;
        }

        public async Task<Cart?> GetByProductId(Guid productId)
        {
            return await _dbContext.Carts.FirstOrDefaultAsync(x => x.ProductId == productId);
        }

        public async Task<Cart> Create(Cart cart)
        {
            await _dbContext.Carts.AddAsync(cart);
            await _dbContext.SaveChangesAsync();

            return cart;
        }

        public async Task<Cart> Update(Cart cart, Cart updatedCart)
        {
            if(updatedCart.ProductCount == 0)
            {
                return await Delete(cart);
            }
            else
            {
                _dbContext.Entry(cart).CurrentValues.SetValues(updatedCart);
                await _dbContext.SaveChangesAsync();
                return updatedCart;
            }
        }

        public async Task<Cart> Delete(Cart cart)
        {
            _dbContext.Carts.Remove(cart);
            await _dbContext.SaveChangesAsync();

            return cart;
        }
    }
}
