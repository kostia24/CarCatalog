using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CarsCatalog.Controllers;
using CarsCatalog.Models;
using CarsCatalog.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarsCatalog.Tests
{
    [TestClass]
    public class BrandTest
    {
        [TestMethod]
        public void GetBrands()
        {
            // arrange
            var brands = new List<CarBrand>
            {
                new CarBrand() {Id = 1, Name = "Audi"},
                new CarBrand() {Id = 2, Name = "BMW"},
                new CarBrand() {Id = 3, Name = "Opel"},
                new CarBrand() {Id = 4, Name = "Mercedes-Benz"}
            }.AsQueryable();

            CarBrand brand = new CarBrand() { Id = 1, Name = "Test" };
            var brandsMock = new Mock<IBrandRepository>();
            brandsMock.Setup(m => m.Update(brand)).Returns(new OperationStatus() { Status = true });

            BrandsController controller = new BrandsController(brandsMock.Object);

            // act
            var result = controller.Edit(brand);

            // Assert
            brandsMock.Verify(m => m.Update(brand));
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));

        }
    }
}
