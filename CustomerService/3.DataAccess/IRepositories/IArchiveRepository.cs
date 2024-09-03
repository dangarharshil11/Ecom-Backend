using CustomerService._3.DataAccess.Domains;

namespace CustomerService._3.DataAccess.IRepositories
{
    public interface IArchiveRepository
    {
        Task<bool> AddProduct(ProductArchive product);
        Task<bool> AddUser(UserArchive user);
        Task<ProductArchive?> GetProduct(Guid Id);
        Task<UserArchive?> GetUser(Guid Id);
    }
}
