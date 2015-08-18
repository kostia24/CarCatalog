using System.Data.Entity;
using CarsCatalog.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CarsCatalog.Repository
{
    public class CatalogDbContext : IdentityDbContext<ApplicationUser>
    {
        public CatalogDbContext()
            : base("name=DbConnection", false) { }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarModel> Models { get; set; }
        public DbSet<CarBrand> Brands { get; set; }
        public DbSet<PriceChangeHistory> PriceChanges { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Car>().ToTable("Cars");
            modelBuilder.Entity<CarBrand>().ToTable("Brands");
            modelBuilder.Entity<CarModel>().ToTable("Models");
        }
        public static CatalogDbContext Create()
        {
            return new CatalogDbContext();
        }
    }
}