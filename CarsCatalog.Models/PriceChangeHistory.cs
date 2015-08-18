using System;
using System.ComponentModel.DataAnnotations;

namespace CarsCatalog.Models
{
    public class PriceChangeHistory
    {
        public int Id { get; set; }

        [DataType(DataType.Date)] 
        public DateTime DateChange { get; set; }
        public int Price { get; set; }
        public int CarId { get; set; }
        public virtual Car Car { get; set; }
    }
}