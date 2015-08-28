using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CarsCatalog.Models;

namespace CarsCatalog.Repository
{
    public class BrandCarRepository : BaseRepository<CatalogDbContext, CarBrand>, IBrandRepository
    {
        public CarBrand GetBrandById(int? id)
        {
            try
            {
                return DataContext.Brands.SingleOrDefault(b => b.Id == id);
            }
            catch (Exception)
            {
                return null;
            }

        }
        public override void Update(CarBrand updatedBrand)
        {
            var brand = GetBrandById(updatedBrand.Id);

            brand.Name = updatedBrand.Name;

            DataContext.Entry(brand).State = EntityState.Modified;
            Save();

        }

        public IList<BrandModelsTree> GetBrandsModelsTree()
        {
            IList<BrandModelsTree> brandsList = new List<BrandModelsTree>();

            using (DataContext)
            {
                try
                {
                    var brands = DataContext.Brands.Include(m => m.Models);
                    foreach (var brand in brands)
                    {
                        BrandModelsTree brandNode = new BrandModelsTree() { Id = brand.Id, Name = brand.Name, Type = "brand" };
                        foreach (var modelNode in brand.Models.Select(model => new BrandModelsTree() { Id = model.Id, Name = model.Name, Type = "model" }))
                        {
                            brandNode.List.Add(modelNode);
                        }
                        brandsList.Add(brandNode);
                    }

                }
                catch (Exception)
                {
                    return null;
                }
            }

            return brandsList;
        }
    }
}