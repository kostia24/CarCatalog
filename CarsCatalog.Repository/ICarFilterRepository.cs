using System;
using System.Collections.Generic;
using System.Linq;
using CarsCatalog.Models;

namespace CarsCatalog.Repository
{
    public interface ICarFiltersRepository
    {
        IQueryable<Car> GetAll();
        IQueryable<Car> GetCarsByModelId(int? modelId);
        IQueryable<Car> GetCarsByBrandId(int? brandId);
        IQueryable<Car> GetCarsFromFilter(FilterParams filter);
        List<Car> GetCarsFromPageRange(IQueryable<Car> cars, int pageNumber, int elementsPerPage);

        IQueryable<Car> GetCarsFromPriceRangeWithSpecificDate(IQueryable<Car> cars, int? minPrice, int? maxPrice,
            DateTime? date);

        IQueryable<Car> GetCarsByEngineCapacity(IQueryable<Car> cars, string capacity);
        IQueryable<Car> GetCarsByColor(IQueryable<Car> cars, string color);
        IQueryable<string> DistinctCarsColor(IQueryable<Car> cars, string startWith);
        IQueryable<string> DistinctCarsEngineCapacity(IQueryable<Car> cars, string startWith);
    }
}