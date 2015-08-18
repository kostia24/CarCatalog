using System.Linq;
using CarsCatalog.Models;

namespace CarsCatalog.Repository
{
    public interface IBrandRepository
    {
        CarBrand GetBrandById(int? id);
        IQueryable<CarBrand> GetAll();
        OperationStatus Add(CarBrand brand);
        OperationStatus Update(CarBrand updatedBrand);
        OperationStatus Delete(CarBrand brand);     
    }
}