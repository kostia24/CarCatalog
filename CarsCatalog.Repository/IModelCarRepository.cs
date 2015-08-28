using System.Collections.Generic;
using System.Linq;
using CarsCatalog.Models;

namespace CarsCatalog.Repository
{
    public interface IModelCarRepository
    {
        CarModel GetModelById(int? id);
        IEnumerable<CarModel> GetModelsByBrandId(int? brandId);
        IQueryable<CarModel> GetAll();

        void Add(CarModel addedModel);
        void Update(CarModel updatedModel);
        void Delete(CarModel deletedModel);
    }
}