using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CarsCatalog.Models;

namespace CarsCatalog.Repository
{
    public class ModelCarRepository : BaseRepository<CatalogDbContext, CarModel>, IModelCarRepository
    {
        public CarModel GetModelById(int? id)
        {
            try
            {
                return DataContext.Models.Include(m => m.Brand).SingleOrDefault(m => m.Id == id);
            }
            catch (Exception)
            {
                return null;
            }

        }
        public IEnumerable<CarModel> GetModelsByBrandId(int? brandId)
        {
            return GetList(car => car.BrandId == brandId);
        }

        public override void Update(CarModel updatedModel)
        {
            var model = GetModelById(updatedModel.Id);

            model.Name = updatedModel.Name;
            model.BrandId = updatedModel.BrandId;
            
            DataContext.Entry(model).State = EntityState.Modified;
            Save();

        }
    }
}