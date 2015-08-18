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

        IEnumerable<Car> GetCarsFromPageRange(IEnumerable<Car> cars, int pageNumber, int elementsPerPage,
            out int pageCount);

        IEnumerable<Car> GetCarsFromPriceRangeWithSpecificDate(IEnumerable<Car> cars, double minPrice, double maxPrice,
            DateTime date);

        IEnumerable<Car> GetCarsByEngineCapacity(IEnumerable<Car> cars, string capacity);
        IEnumerable<Car> GetCarsByColor(IEnumerable<Car> cars, string color);
        IEnumerable<string> DistinctCarsColor(IEnumerable<Car> cars, string startWith);
        IEnumerable<string> DistinctCarsEngineCapacity(IEnumerable<Car> cars, string startWith);
    }
}