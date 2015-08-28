using System.Collections.Generic;
using System.Linq;
using CarsCatalog.Models;

namespace CarsCatalog.Repository
{
    public interface IBrandRepository
    {
        CarBrand GetBrandById(int? id);
        IQueryable<CarBrand> GetAll();
        void Add(CarBrand brand);
        void Update(CarBrand updatedBrand);
        void Delete(CarBrand brand);
        IList<BrandModelsTree> GetBrandsModelsTree();
    }
}