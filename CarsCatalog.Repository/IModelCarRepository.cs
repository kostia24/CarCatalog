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

        OperationStatus Add(CarModel updatedModel);
        OperationStatus Update(CarModel updatedModel);
        OperationStatus Delete(CarModel updatedModel);
    }
}