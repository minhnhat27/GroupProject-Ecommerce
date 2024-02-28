using GroupProject_Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace GroupProject_Ecommerce.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UsersRoles { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Address> Addresses { get; set; }
    }
}
