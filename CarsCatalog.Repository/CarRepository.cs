using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CarsCatalog.Models;

namespace CarsCatalog.Repository
{
    public class CarRepository : BaseRepository<CatalogDbContext, Car>, ICarFiltersRepository, ICarRepository
    {
        private readonly ChangePriceRepository _priceRepository = new ChangePriceRepository();

        public Car GetCarById(int? id)
        {
            try
            {
                return
                    DataContext.Cars.Include(c => c.Model).Include(m => m.Model.Brand).SingleOrDefault(c => c.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public OperationStatus AddCarWithDate(Car car, DateTime date)
        {
            _priceRepository.Add(new PriceChangeHistory()
            {
                CarId = car.Id,
                DateChange = date,
                Price = car.Price
            });
            return Add(car);
        }

        public override OperationStatus Update(Car updatedCar)
        {
            var opStatus = new OperationStatus { Status = true };
            Car car = GetCarById(updatedCar.Id);

            car.EngineCapacity = updatedCar.EngineCapacity;
            car.Color = updatedCar.Color;
            car.Description = updatedCar.Description;
            car.ModelId = updatedCar.ModelId;
            car.ImageData = updatedCar.ImageData;
            car.ImageMimeType = updatedCar.ImageMimeType;

            if (updatedCar.Price != car.Price)
                _priceRepository.Add(new PriceChangeHistory()
                {
                    CarId = car.Id,
                    DateChange = DateTime.Today,
                    Price = updatedCar.Price
                });
            car.Price = updatedCar.Price;

            try
            {
                DataContext.Entry(car).State = EntityState.Modified;
                Save();
            }
            catch (Exception exp)
            {
                opStatus = OperationStatus.CreateFromExeption("Error updating car.", exp);
            }
            return opStatus;
        }

        public IQueryable<Car> GetCarsByModelId(int? modelId)
        {
            return GetList(car => car.ModelId == modelId);
        }

        public IQueryable<Car> GetCarsByBrandId(int? brandId)
        {
            return GetList(car => car.Model.BrandId == brandId);
        }

        public IEnumerable<Car> GetCarsFromPageRange(IEnumerable<Car> cars, int pageNumber, int elementsPerPage,
            out int pageCount)
        {
            pageNumber--;
            int count = cars.Count();

            if (count % elementsPerPage == 0)
                pageCount = count / elementsPerPage;
            else
            {
                pageCount = (count / elementsPerPage);
                pageCount++;
            }
            return
                cars.OrderBy(car => car.Id)
                    .Skip(pageNumber * elementsPerPage)
                    .Take(elementsPerPage);
        }

        public IEnumerable<Car> GetCarsFromPriceRangeWithSpecificDate(IEnumerable<Car> cars, double minPrice,
            double maxPrice, DateTime date)
        {
            date += new TimeSpan(1, 0, 0, 0);
            var carsWithSelectedPrice = cars.Where(
                c =>
                {
                    if (!c.PriceChangeHistories.Any()) return false;

                    var carPrices = c.PriceChangeHistories.OrderByDescending(d => d.DateChange)
                        .FirstOrDefault(history => history.DateChange < date);
                    if (carPrices == null)
                        return false;
                    if (minPrice < carPrices.Price && carPrices.Price < maxPrice)
                        return true;
                    return false;
                });

            return carsWithSelectedPrice;
        }

        public IEnumerable<Car> GetCarsByEngineCapacity(IEnumerable<Car> cars, string capacity)
        {
            return cars.Where(car => car.EngineCapacity.StartsWith(capacity));
        }

        public IEnumerable<Car> GetCarsByColor(IEnumerable<Car> cars, string color)
        {
            return cars.Where(car => car.Color.StartsWith(color, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<string> DistinctCarsColor(IEnumerable<Car> cars, string startWith)
        {
            return
                cars.Select(c => c.Color)
                    .Where(c => c.StartsWith(startWith, StringComparison.OrdinalIgnoreCase))
                    .Distinct();
        }

        public IEnumerable<string> DistinctCarsEngineCapacity(IEnumerable<Car> cars, string startWith)
        {
            return
                cars.Select(c => c.EngineCapacity)
                    .Where(c => c.StartsWith(startWith, StringComparison.OrdinalIgnoreCase))
                    .Distinct();
        }
    }
}