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

        public override OperationStatus Update(CarModel updatedModel)
        {
            var model = GetModelById(updatedModel.Id);
            var opStatus = new OperationStatus { Status = true };

            model.Name = updatedModel.Name;
            model.BrandId = updatedModel.BrandId;

            try
            {
                DataContext.Entry(model).State = EntityState.Modified;
                Save();
            }
            catch (Exception exp)
            {
                opStatus = OperationStatus.CreateFromExeption("Error updating model car.", exp);
            }
            return opStatus;
        }
    }
}