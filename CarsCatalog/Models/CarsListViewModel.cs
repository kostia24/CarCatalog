using System.Collections.Generic;

namespace CarsCatalog.Models
{
    public class CarsListViewModel
    {
        public IEnumerable<Car> Cars { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public FilterParams FilterParams { get; set; }
    }
}
