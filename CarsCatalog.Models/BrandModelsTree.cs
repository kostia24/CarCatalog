using System.Collections.Generic;

namespace CarsCatalog.Models
{

    public class BrandModelsTree
    {
        public List<BrandModelsTree> List { get; private set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        
 
        public BrandModelsTree()
        {
            List = new List<BrandModelsTree>();
        } 
    }
}