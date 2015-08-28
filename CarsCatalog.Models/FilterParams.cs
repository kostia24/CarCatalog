using System;

namespace CarsCatalog.Models
{
    public class FilterParams
    {
        public int? BrandId { get; set; }
        public int? ModelId { get; set; }
        public string Color { get; set; }
        public string Engine { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public DateTime? Date { get; set; }
    }
}
