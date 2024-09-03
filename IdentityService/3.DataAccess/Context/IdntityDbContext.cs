using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityService._3.DataAccess.Domains;
using IdentityService._4.Infrastructure.Constants;

namespace IdentityService._3.DataAccess.Context
{
    public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        // Initalizing the Auth Database with Roles and Admin User
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Initializing Default Roles
            List<IdentityRole> roles = [
                new()
                {
                    Id = Constants.ADMIN_ID,
                    Name = Constants.ADMIN_NAME,
                    NormalizedName = Constants.ADMIN_NAME.ToUpper(),
                    ConcurrencyStamp = Constants.ADMIN_ID
                },
                new()
                {
                    Id = Constants.CUSTOMER_ID,
                    Name = Constants.CUSTOMER_NAME,
                    NormalizedName = Constants.CUSTOMER_NAME.ToUpper(),
                    ConcurrencyStamp = Constants.CUSTOMER_ID
                }
            ];

            builder.Entity<IdentityRole>().HasData(roles);

            // Initializing Default Admin User
            ApplicationUser admin = new()
            {
                Id = Constants.ADMIN_USER_ID,
                UserName = Constants.ADMIN_USER_USERNAME,
                FirstName = Constants.ADMIN_USER_FIRSTNAME,
                LastName = Constants.ADMIN_USER_LASTNAME,
                Address = "Demo address of the admin",
                Email = Constants.ADMIN_USER_EMAIL,
                NormalizedEmail = Constants.ADMIN_USER_EMAIL.ToUpper(),
                NormalizedUserName = Constants.ADMIN_USER_USERNAME.ToUpper(),
                PhoneNumber = Constants.ADMIN_USER_PHONENUMBER,
            };

            admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, Constants.ADMIN_USER_PASSWORD);

            builder.Entity<ApplicationUser>().HasData(admin);

            // Initializing Default Admin User Role
            List<IdentityUserRole<string>> adminRole = [
                new()
                {
                    UserId = Constants.ADMIN_USER_ID,
                    RoleId = Constants.ADMIN_ID
                },
            ];

            builder.Entity<IdentityUserRole<string>>().HasData(adminRole);
        }

    }
}
