using System.Collections.Generic;
using CarsCatalog.Models;

namespace CarsCatalog.Repository
{
    public interface IBrandCarTreeRepository
    {
        IList<BrandModelsTree> GetBrandsModelsTree();

    }
}