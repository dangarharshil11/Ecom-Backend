using ProductService._2.BusinessLogic.DTO;
using ProductService._3.DataAccess.Domains;

namespace ProductService._3.DataAccess.IRepositories
{
    public interface IProductRepository
    {
        Task<Product> CreateProduct(Product product);
        Task<Product> DeleteProduct(Product product);
        Task<Product?> GetProductById(Guid Id);
        Task<Product?> GetProductByName(string Name);
        Task<Dictionary<Guid, string>> GetProductList();
        Task<List<Product>> GetProducts(List<Guid> productIds);
        Task<List<Product>> GetProductsAsync(FilterModel filter);
        Task<List<Product>> GetProductsByCategory(Guid categoryId);
        Task<Product> UpdateProduct(Product Product, Product updatedProduct);
    }
}
