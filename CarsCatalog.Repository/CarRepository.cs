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
            return DataContext.Cars.Include(c => c.Model).Include(m => m.Model.Brand).SingleOrDefault(c => c.Id == id);
        }

        public IQueryable<Car> GetCarsByUserId(string userId)
        {
            return GetList(car => car.UserId == userId);
        }

        public override void Add(Car car)
        {
            base.Add(car);
            _priceRepository.Add(new PriceChangeHistory()
            {
                CarId = car.Id,
                DateChange = DateTime.Today,
                Price = car.Price
            });
        }

        public override void Update(Car updatedCar)
        {
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

            DataContext.Entry(car).State = EntityState.Modified;
            Save();
        }

        public IQueryable<Car> GetCarsFromFilter(FilterParams filter)
        {
            IQueryable<Car> cars;
            if (!filter.BrandId.HasValue && !filter.ModelId.HasValue)
                cars = GetAll();
            else
                cars = filter.ModelId.HasValue ? GetCarsByModelId(filter.ModelId) : GetCarsByBrandId(filter.BrandId);

            if (filter.Color != null)
                cars = GetCarsByColor(cars, filter.Color);

            if (filter.Engine != null)
                cars = GetCarsByEngineCapacity(cars, filter.Engine);
            if (filter.MinPrice.HasValue || filter.MaxPrice.HasValue || filter.Date.HasValue)
            {
                int min = 0;
                int max = int.MaxValue;
                DateTime date = DateTime.Today;
                if (filter.MinPrice > 0)
                    if (filter.MinPrice != null) min = filter.MinPrice.Value;
                if (filter.MaxPrice > 0)
                    if (filter.MaxPrice != null) max = filter.MaxPrice.Value;
                if (filter.Date.HasValue)
                    date = filter.Date.Value;
                cars = GetCarsFromPriceRangeWithSpecificDate(cars, min, max, date);
            }
            return cars;

        }
        public IQueryable<Car> GetCarsByModelId(int? modelId)
        {
            return GetList(car => car.ModelId == modelId);
        }

        public IQueryable<Car> GetCarsByBrandId(int? brandId)
        {
            return GetList(car => car.Model.BrandId == brandId);
        }

        public List<Car> GetCarsFromPageRange(IQueryable<Car> cars, int pageNumber, int elementsPerPage)
        {
            pageNumber--;
            return cars.OrderBy(car => car.Id)
                    .Skip(pageNumber * elementsPerPage)
                    .Take(elementsPerPage).ToList();
        }

        public IQueryable<Car> GetCarsFromPriceRangeWithSpecificDate(IQueryable<Car> cars, int? minPrice,
            int? maxPrice, DateTime? date)
        {
            date += new TimeSpan(1, 0, 0, 0);
            var carsWithSelectedPrice = cars.Where(c => c.PriceChangeHistories.OrderByDescending(d => d.DateChange)
                .FirstOrDefault(history => history.DateChange < date).Price > minPrice);
            carsWithSelectedPrice = carsWithSelectedPrice.Where(c => c.PriceChangeHistories.OrderByDescending(d => d.DateChange)
                .FirstOrDefault(history => history.DateChange < date).Price < maxPrice);
            return carsWithSelectedPrice;
        }

        public IQueryable<Car> GetCarsByEngineCapacity(IQueryable<Car> cars, string capacity)
        {
            return cars.Where(car => car.EngineCapacity.StartsWith(capacity));
        }

        public IQueryable<Car> GetCarsByColor(IQueryable<Car> cars, string color)
        {
            return cars.Where(car => car.Color.StartsWith(color));
        }

        public IQueryable<string> DistinctCarsColor(IQueryable<Car> cars, string startWith)
        {
            return
                cars.Select(c => c.Color)
                    .Where(c => c.StartsWith(startWith))
                    .Distinct();
        }

        public IQueryable<string> DistinctCarsEngineCapacity(IQueryable<Car> cars, string startWith)
        {
            return
                cars.Select(c => c.EngineCapacity)
                    .Where(c => c.StartsWith(startWith))
                    .Distinct();
        }
    }
}