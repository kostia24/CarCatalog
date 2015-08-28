using System;
using System.Collections.Generic;
using System.Linq;
using CarsCatalog.Models;

namespace CarsCatalog.Repository
{
    public class ChangePriceRepository : BaseRepository<CatalogDbContext, PriceChangeHistory>, IPriceRepository
    {
        public IEnumerable<PriceChangeHistory> GetListPriceCars(IEnumerable<Car> cars, int? minPrice, int? maxPrice, DateTime date )
        {
            date += new TimeSpan(1, 0, 0, 0);
            if (!minPrice.HasValue)
                minPrice = 0;
            if (!maxPrice.HasValue)
                maxPrice = int.MaxValue;

            return cars.Select(
                    c =>
                        c.PriceChangeHistories.OrderByDescending(pr=>pr.DateChange).First(
                            p => p.DateChange < date && minPrice < p.Price && p.Price < maxPrice));
        }

        public List<PriceChangeHistory> GetChangePriceForCar(int? carId)
        {
            return GetList(h => h.CarId == carId).ToList();
        }
    }
}