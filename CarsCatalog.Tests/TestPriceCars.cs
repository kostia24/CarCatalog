using System;
using System.Collections.Generic;
using System.Linq;
using CarsCatalog.Models;
using CarsCatalog.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarsCatalog.Tests
{
    [TestClass]
    public class TestPriceCars
    {
        private IPriceRepository GetTarget()
        {
            return new ChangePriceRepository();
        }
        private ICarFiltersRepository GetCarTarget()
        {
            return new CarRepository();
        }
        [TestMethod]
        public void TestPriceCar()
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
                        new PriceChangeHistory(){Price = 23000, CarId = 2 , DateChange = new DateTime(2015, 8, 3)},
                        new PriceChangeHistory(){Price = 24000, CarId = 2 , DateChange = new DateTime(2015, 8, 4)},
                        new PriceChangeHistory(){Price = 25000, CarId = 2 , DateChange = new DateTime(2015, 8, 5)}
                    }
                },
                new Car()
                {
                    Price = 32200, EngineCapacity = "1.5", Color = "White", ModelId = 14,
                     PriceChangeHistories = new List<PriceChangeHistory>
                    {
                        new PriceChangeHistory(){Price = 34000, CarId = 3 , DateChange = new DateTime(2015, 8, 4)},
                        new PriceChangeHistory(){Price = 36000, CarId = 3 , DateChange = new DateTime(2015, 8, 6)},
                        new PriceChangeHistory(){Price = 37000, CarId = 3 , DateChange = new DateTime(2015, 8, 7)}
                    }
                },
                new Car()
                {
                    Price = 42300, EngineCapacity = "2.4", Color = "Red", ModelId = 2,
                     PriceChangeHistories = new List<PriceChangeHistory>
                    {
                        new PriceChangeHistory(){Price = 44000, CarId = 4 , DateChange = new DateTime(2015, 8, 4)},
                        new PriceChangeHistory(){Price = 45000, CarId = 4 , DateChange = new DateTime(2015, 8, 5)},
                        new PriceChangeHistory(){Price = 46000, CarId = 4 , DateChange = new DateTime(2015, 8, 6)}
                    }
                },
                new Car()
                {
                    Price = 52400, EngineCapacity = "1.6", Color = "Blue", ModelId = 2, 
                     PriceChangeHistories = new List<PriceChangeHistory>
                    {
                        new PriceChangeHistory(){Price = 51000, CarId = 5 , DateChange = new DateTime(2015, 8, 1)},
                        new PriceChangeHistory(){Price = 52000, CarId = 5 , DateChange = new DateTime(2015, 8, 2)},
                        new PriceChangeHistory(){Price = 55000, CarId = 5 , DateChange = new DateTime(2015, 8, 5)}
                    }
                },
            }.AsQueryable();
            var target = GetTarget();
            var carInput = GetCarTarget();
            // act
            var resultcarsAll = carInput.GetCarsFromPriceRangeWithSpecificDate(cars, 0, 57000, new DateTime(2015, 8, 8));
            var resultcarsWithLastDateAndLimitUpperPrice = carInput.GetCarsFromPriceRangeWithSpecificDate(cars, 0, 52000, new DateTime(2015, 8, 8));
            var resultcarsWithMiddlePriceRange = carInput.GetCarsFromPriceRangeWithSpecificDate(cars, 35000, 52000, new DateTime(2015, 8, 5));
            var resultcarsMiddleDate = carInput.GetCarsFromPriceRangeWithSpecificDate(cars, 25000, 59000, new DateTime(2015, 8, 4));

            var resultPricecarsAll = target.GetListPriceCars(resultcarsAll, 0, 57000, new DateTime(2015, 8, 8));
            var resultcarsPriceWithLastDateAndLimitUpperPrice = target.GetListPriceCars(resultcarsWithLastDateAndLimitUpperPrice, 0, 52000, new DateTime(2015, 8, 8)).ToList();
            var resultPricecarsWithMiddlePriceRange = target.GetListPriceCars(resultcarsWithMiddlePriceRange, 35000, 52000, new DateTime(2015, 8, 5)).ToList();
            var resultPricecarsMiddleDate = target.GetListPriceCars(resultcarsMiddleDate, 25000, 59000, new DateTime(2015, 8, 4));
            // assert
            Assert.AreEqual(5, resultPricecarsAll.Count());

            Assert.AreEqual(4, resultcarsPriceWithLastDateAndLimitUpperPrice.Count);
            Assert.AreEqual(46000, resultcarsPriceWithLastDateAndLimitUpperPrice[3].Price);

            Assert.AreEqual(1, resultPricecarsWithMiddlePriceRange.Count);
            Assert.AreEqual(45000, resultPricecarsWithMiddlePriceRange[0].Price);
            Assert.AreEqual(3, resultPricecarsMiddleDate.Count());

        }
    }
}
