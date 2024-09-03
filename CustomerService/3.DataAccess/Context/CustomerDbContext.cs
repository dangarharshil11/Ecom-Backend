using CustomerService._3.DataAccess.Domains;
using Microsoft.EntityFrameworkCore;

namespace CustomerService._3.DataAccess.Context
{
    public class CustomerDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ProductArchive> ProductArchives { get; set; }
        public DbSet<CategoryArchive> CategoryArchives { get; set; }
        public DbSet<UserArchive> UserArchives { get; set; }

    }
}
