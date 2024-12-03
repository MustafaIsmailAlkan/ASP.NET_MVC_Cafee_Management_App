using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cafee_Prototype.Models
{
    public class CafeeDbContext : IdentityDbContext<IdentityUser>
    {
        public CafeeDbContext(DbContextOptions<CafeeDbContext> options) : base(options)
        {
            
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Product>().HasData(
                new List<Product>()
                {
                    new Product {ProductId=1 ,ProductName="Mercimek Çorbası", ProductDescription="Güzel bir çorba", ProductPrice=30, ProductImage="mercimek_corbasi.jpg", IsActive = true, ProductCategoryId = 3},
                    new Product {ProductId=2 ,ProductName="Menemen", ProductDescription="Güzel bir Menemen", ProductPrice=50, ProductImage="menemen.jpg", IsActive = true, ProductCategoryId = 2},
                    new Product {ProductId=3 ,ProductName="Ayran", ProductDescription="Güzel bir Ayran", ProductPrice=10, ProductImage="ayran.jpg", IsActive = true, ProductCategoryId = 4},
                    new Product {ProductId=4 ,ProductName="Urfa Kebap", ProductDescription="Güzel bir Urfa Kebap", ProductPrice=100, ProductImage="urfa_kebap.jpg", IsActive = true, ProductCategoryId = 1},
                    new Product {ProductId=5 ,ProductName="Adana Kebap", ProductDescription="Güzel bir Adana Kebap", ProductPrice=110, ProductImage="adana_kebap.jpg", IsActive = false, ProductCategoryId = 1}

                }
            );

            modelBuilder.Entity<ProductCategory>().HasData(
                new List<ProductCategory>()
                {
                    new ProductCategory { ProductCategoryId = 1, CategoryName="Ana Yemek"},
                    new ProductCategory { ProductCategoryId = 2, CategoryName="Ara Sıcak"},
                    new ProductCategory { ProductCategoryId = 3, CategoryName="Çorba"},
                    new ProductCategory { ProductCategoryId = 4, CategoryName="İçecek"},
                }
            );

            //Identity    
            var adminUser = new IdentityUser
            {
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
            };
            adminUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(adminUser ,"admin123");

            modelBuilder.Entity<IdentityUser>().HasData(
                adminUser
            );
            
            //Order
            modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(o => o.Order)
            .HasForeignKey(o => o.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        }

        public void Add(Product product)
        {
            Products.Add(product);
        }
    }
}