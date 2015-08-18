using System;
using System.Collections.Generic;
using System.Linq;
using CarsCatalog.Models;
using CarsCatalog.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarsCatalog.Tests
{
    [TestClass]
    public class CarFiltersTest
    {
        private ICarFiltersRepository GetTarget()
        {
            return new CarRepository();
        }


        [TestMethod]
        public void GetCarsByEngineCapacityTest()
        {
            // arrange
            var cars = new List<Car>
            {
                new Car() {Price = 12000, EngineCapacity = "2.5", Color = "Black", ModelId = 2},
                new Car() {Price = 12100, EngineCapacity = "2.5", Color = "Black", ModelId = 3},
                new Car() {Price = 12200, EngineCapacity = "1.5", Color = "White", ModelId = 14},
                new Car() {Price = 12300, EngineCapacity = "2.4", Color = "Red", ModelId = 2},
                new Car() {Price = 12400, EngineCapacity = "1.6", Color = "Blue", ModelId = 2},
                new Car() {Price = 7500, EngineCapacity = "2.5", Color = "Blue", ModelId = 13},
                new Car() {Price = 4500, EngineCapacity = "2.5", Color = "Blue", ModelId = 7}
            };
            var target = GetTarget();
            // act
            var resultcars = target.GetCarsByEngineCapacity(cars, "1.");
            // assert
            Assert.AreEqual(2, resultcars.Count());

        }
        [TestMethod]
        public void GetCarsByEngineCapacityNoResultsTest()
        {
            // arrange
            var cars = new List<Car>
            {
                new Car() {Price = 12000, EngineCapacity = "2.5", Color = "Black", ModelId = 2},
                new Car() {Price = 12100, EngineCapacity = "2.5", Color = "Black", ModelId = 3},
                new Car() {Price = 12200, EngineCapacity = "1.5", Color = "White", ModelId = 14},
                new Car() {Price = 12300, EngineCapacity = "2.4", Color = "Red", ModelId = 2},
                new Car() {Price = 12400, EngineCapacity = "1.6", Color = "Blue", ModelId = 2},
                new Car() {Price = 7500, EngineCapacity = "2.5", Color = "Blue", ModelId = 13},
                new Car() {Price = 4500, EngineCapacity = "2.5", Color = "Blue", ModelId = 7}
            };
            var target = GetTarget();
            // act
            var resultcars = target.GetCarsByEngineCapacity(cars, "4");
            // assert
            Assert.AreEqual(0, resultcars.Count());

        }

        [TestMethod]
        public void GetCarsByPriceWithDate()
        {
            // arrange
            var cars = new List<Car>
            {
                new Car()
                {
                    Price = 11000, EngineCapacity = "2.5", Color = "Black", ModelId = 2, 
                    PriceChangeHistories = new List<PriceChangeHistory>
                    {
                        new PriceChangeHistory(){Price = 12000, CarId = 1 , DateChange = new DateTime(2015, 8, 2)},
                        new PriceChangeHistory(){Price = 13000, CarId = 1 , DateChange = new DateTime(2015, 8, 3)},
                        new PriceChangeHistory(){Price = 14000, CarId = 1 , DateChange = new DateTime(2015, 8, 4)}
                    }
                    
                },
                new Car()
                {
                    Price = 22100, EngineCapacity = "2.5", Color = "Black", ModelId = 3,
                     PriceChangeHistories = new List<PriceChangeHistory>
                    {
                        new PriceChangeHistory(){Price = 23000, CarId = 1 , DateChange = new DateTime(2015, 8, 3)},
                        new PriceChangeHistory(){Price = 24000, CarId = 1 , DateChange = new DateTime(2015, 8, 4)},
                        new PriceChangeHistory(){Price = 25000, CarId = 1 , DateChange = new DateTime(2015, 8, 5)}
                    }
                },
                new Car()
                {
                    Price = 32200, EngineCapacity = "1.5", Color = "White", ModelId = 14,
                     PriceChangeHistories = new List<PriceChangeHistory>
                    {
                        new PriceChangeHistory(){Price = 34000, CarId = 2 , DateChange = new DateTime(2015, 8, 4)},
                        new PriceChangeHistory(){Price = 36000, CarId = 2 , DateChange = new DateTime(2015, 8, 6)},
                        new PriceChangeHistory(){Price = 37000, CarId = 2 , DateChange = new DateTime(2015, 8, 7)}
                    }
                },
                new Car()
                {
                    Price = 42300, EngineCapacity = "2.4", Color = "Red", ModelId = 2,
                     PriceChangeHistories = new List<PriceChangeHistory>
                    {
                        new PriceChangeHistory(){Price = 44000, CarId = 2 , DateChange = new DateTime(2015, 8, 4)},
                        new PriceChangeHistory(){Price = 45000, CarId = 2 , DateChange = new DateTime(2015, 8, 5)},
                        new PriceChangeHistory(){Price = 46000, CarId = 2 , DateChange = new DateTime(2015, 8, 6)}
                    }
                },
                new Car()
                {
                    Price = 52400, EngineCapacity = "1.6", Color = "Blue", ModelId = 2, 
                     PriceChangeHistories = new List<PriceChangeHistory>
                    {
                        new PriceChangeHistory(){Price = 51000, CarId = 2 , DateChange = new DateTime(2015, 8, 1)},
                        new PriceChangeHistory(){Price = 52000, CarId = 2 , DateChange = new DateTime(2015, 8, 2)},
                        new PriceChangeHistory(){Price = 55000, CarId = 2 , DateChange = new DateTime(2015, 8, 5)}
                    }
                },
            };
            var target = GetTarget();
            // act
            var resultcarsAll = target.GetCarsFromPriceRangeWithSpecificDate(cars, 0, 57000, new DateTime(2015, 8, 8));
            var resultcarsWithLastDateAndLimitUpperPrice = target.GetCarsFromPriceRangeWithSpecificDate(cars, 0, 52000, new DateTime(2015, 8, 8));
            var resultcarsWithMiddlePriceRange = target.GetCarsFromPriceRangeWithSpecificDate(cars, 35000, 52000, new DateTime(2015, 8, 5));
            var resultcarsEmpty = target.GetCarsFromPriceRangeWithSpecificDate(cars, 45000, 52000, new DateTime(2015, 8, 3));
            var resultcarsMiddleDate = target.GetCarsFromPriceRangeWithSpecificDate(cars, 25000, 59000, new DateTime(2015, 8, 4));
            // assert
            Assert.AreEqual(5, resultcarsAll.Count());
            Assert.AreEqual(4, resultcarsWithLastDateAndLimitUpperPrice.Count());
            Assert.AreEqual(1, resultcarsWithMiddlePriceRange.Count());
            Assert.AreEqual(0, resultcarsEmpty.Count());
            Assert.AreEqual(3, resultcarsMiddleDate.Count());


        }
    }
}
