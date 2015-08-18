using System;
using System.Collections.Generic;
using CarsCatalog.Models;

namespace CarsCatalog.Repository
{
    public interface IPriceRepository
    {
        List<PriceChangeHistory> GetChangePriceForCar(int? carId);
        IEnumerable<PriceChangeHistory> GetListPriceCars(IEnumerable<Car> cars, int minPrice, int maxPrice, DateTime date);
    }
}