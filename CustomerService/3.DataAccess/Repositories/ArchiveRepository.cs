using CustomerService._3.DataAccess.Context;
using CustomerService._3.DataAccess.Domains;
using CustomerService._3.DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CustomerService._3.DataAccess.Repositories
{
    public class ArchiveRepository(CustomerDbContext dbContext) : IArchiveRepository
    {
        private readonly CustomerDbContext _dbContext = dbContext;

        public async Task<bool> AddProduct(ProductArchive product)
        {
            if(product == null)
            {
                return false;
            }
            else
            {
                ProductArchive? productArchive = await _dbContext.ProductArchives.AsNoTracking().FirstOrDefaultAsync(x => x.Id == product.Id);

                if(productArchive == null)
                {
                    CategoryArchive? categoryArchive = await _dbContext.CategoryArchives.AsNoTracking().FirstOrDefaultAsync(x => x.Id == product.CategoryId);
                    if(categoryArchive == null && product.Category != null)
                    {
                        await _dbContext.CategoryArchives.AddAsync(product.Category);
                        await _dbContext.SaveChangesAsync();
                    }
                    product.Category = null;
                    await _dbContext.ProductArchives.AddAsync(product);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
        }

        public async Task<ProductArchive?> GetProduct(Guid Id)
        {
            return await _dbContext.ProductArchives.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<bool> AddUser(UserArchive user)
        {
            if(user == null)
            {
                return false;
            }
            else
            {
                UserArchive? userArchive = await _dbContext.UserArchives.FirstOrDefaultAsync(x => x.Id == user.Id);

                if (userArchive == null)
                {
                    await _dbContext.UserArchives.AddAsync(user);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
        }
        public async Task<UserArchive?> GetUser(Guid Id)
        {
            return await _dbContext.UserArchives.FirstOrDefaultAsync(x => x.Id == Id);
        }
    }
}
