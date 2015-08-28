using System;
using System.Data.Entity;
using CarsCatalog.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CarsCatalog.Models
{
    public class CatalogDbInitializer : DropCreateDatabaseIfModelChanges<CatalogDbContext>
    {
        protected override void Seed(CatalogDbContext context)
        {
            context.Brands.Add(new CarBrand() { Id = 1, Name = "Audi" });
            context.Brands.Add(new CarBrand() { Id = 2, Name = "BMW" });
            context.Brands.Add(new CarBrand() { Id = 3, Name = "Opel" });
            context.Brands.Add(new CarBrand() { Id = 4, Name = "Mercedes-Benz" });

            context.Models.Add(new CarModel() { Id = 1, Name = "A4", BrandId = 1 });
            context.Models.Add(new CarModel() { Id = 2, Name = "A6", BrandId = 1 });
            context.Models.Add(new CarModel() { Id = 3, Name = "A8", BrandId = 1 });
            context.Models.Add(new CarModel() { Id = 4, Name = "100", BrandId = 1 });
            context.Models.Add(new CarModel() { Id = 5, Name = "200", BrandId = 1 });
            context.Models.Add(new CarModel() { Id = 6, Name = "X6", BrandId = 2 });
            context.Models.Add(new CarModel() { Id = 7, Name = "X6", BrandId = 2 });
            context.Models.Add(new CarModel() { Id = 8, Name = "520", BrandId = 2 });
            context.Models.Add(new CarModel() { Id = 9, Name = "730", BrandId = 2 });
            context.Models.Add(new CarModel() { Id = 10, Name = "Vivaro", BrandId = 3 });
            context.Models.Add(new CarModel() { Id = 11, Name = "Omega", BrandId = 3 });
            context.Models.Add(new CarModel() { Id = 12, Name = "Vectra", BrandId = 3 });
            context.Models.Add(new CarModel() { Id = 13, Name = "Vito", BrandId = 4 });
            context.Models.Add(new CarModel() { Id = 14, Name = "E-class", BrandId = 4 });

            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context));

            var roleAdmin = new IdentityRole { Name = "admin" };
            var roleUser = new IdentityRole { Name = "user" };

            roleManager.Create(roleAdmin);
            roleManager.Create(roleUser);

            var admin = new ApplicationUser { Email = "admin@mail.com", UserName = "admin@mail.com" };
            var testUser = new ApplicationUser { Email = "test@mail.com", UserName = "test@mail.com" };
            const string password = "qwerty123456";
            var result = userManager.Create(admin, password);

            if (result.Succeeded)
            {
                userManager.AddToRole(admin.Id, roleAdmin.Name);
                userManager.AddToRole(admin.Id, roleUser.Name);
            }

            var resultUser = userManager.Create(testUser, password);

            if (resultUser.Succeeded)
            {
                userManager.AddToRole(testUser.Id, roleUser.Name);
            }

            context.Cars.Add(new Car() { UserId = testUser.Id, Price = 12000, EngineCapacity = "2.5", Color = "Black", ModelId = 2 });
            context.Cars.Add(new Car() { UserId = testUser.Id, Price = 12000, EngineCapacity = "2.5", Color = "Black", ModelId = 2 });
            context.Cars.Add(new Car() { UserId = testUser.Id, Price = 12100, EngineCapacity = "2.5", Color = "Black", ModelId = 3 });
            context.Cars.Add(new Car() { UserId = testUser.Id, Price = 12200, EngineCapacity = "1.5", Color = "White", ModelId = 14 });
            context.Cars.Add(new Car() { UserId = testUser.Id, Price = 12300, EngineCapacity = "2.4", Color = "Red", ModelId = 2 });
            context.Cars.Add(new Car() { UserId = testUser.Id, Price = 12400, EngineCapacity = "1.6", Color = "Blue", ModelId = 2 });
            context.Cars.Add(new Car() { UserId = testUser.Id, Price = 7500, EngineCapacity = "2.5", Color = "Blue", ModelId = 13 });
            context.Cars.Add(new Car() { UserId = testUser.Id, Price = 4500, EngineCapacity = "2.5", Color = "Blue", ModelId = 7 });


            Random random = new Random();
            for (int i = 8; i < 40; i++)
            {
                var car = new Car()
                {
                    Price = 500 + 100 * i,
                    EngineCapacity = random.Next(1, 10) + "." + random.Next(1, 10),
                    Color = random.Next(2) == 1 ? "Black" : random.Next(2) != 1 ? "Blue" : "Red",
                    ModelId = random.Next(1, 14),
                    UserId = testUser.Id
                };
                context.Cars.Add(car);
                context.PriceChanges.Add(new PriceChangeHistory() { CarId = i, Price = 1000 + 100 * i, DateChange = new DateTime(2015, 8, random.Next(1, 30)) });
            }

            for (int i = 0; i < 20; i++)
            {
                context.PriceChanges.Add(new PriceChangeHistory() { CarId = i + 1, Price = 5500 + 1000 * i, DateChange = new DateTime(2015, 8, random.Next(1, 30)) });
                context.PriceChanges.Add(new PriceChangeHistory() { CarId = i + 4, Price = 4500 + 1000 * i, DateChange = new DateTime(2015, 8, random.Next(1, 30)) });
            }

            context.PriceChanges.Add(new PriceChangeHistory() { CarId = 1, Price = 10000, DateChange = new DateTime(2015, 8, 1) });
            context.PriceChanges.Add(new PriceChangeHistory() { CarId = 1, Price = 11000, DateChange = new DateTime(2015, 8, 2) });
            context.PriceChanges.Add(new PriceChangeHistory() { CarId = 1, Price = 12000, DateChange = new DateTime(2015, 8, 3) });
            context.PriceChanges.Add(new PriceChangeHistory() { CarId = 2, Price = 21000, DateChange = new DateTime(2015, 8, 2) });
            context.PriceChanges.Add(new PriceChangeHistory() { CarId = 2, Price = 23000, DateChange = new DateTime(2015, 8, 4) });
            context.PriceChanges.Add(new PriceChangeHistory() { CarId = 3, Price = 33000, DateChange = new DateTime(2015, 8, 4) });



            base.Seed(context);
        }
    }
}