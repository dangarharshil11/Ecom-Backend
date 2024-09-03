using Microsoft.EntityFrameworkCore;
using ProductService._3.DataAccess.Domains;

namespace ProductService._3.DataAccess.Context
{
    public class ProductDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<StockLevel> StockLevels { get; set; }
    }
}
