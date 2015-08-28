using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarsCatalog.Models
{
    public class CarBrand
    {
        [Required]
        public int Id{ get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Brand")]
        public string Name{ get; set; }
        public virtual ICollection<CarModel> Models{ get; set; }
    }
}