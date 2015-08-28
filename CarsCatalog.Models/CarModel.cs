using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarsCatalog.Models
{
    public class CarModel
    {
        [Required]
        public int Id{ get; set; }
        [Display(Name = "Model")]
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name{ get; set; }
        [Required]
        public int BrandId{ get; set; }
        public virtual CarBrand Brand{ get; set; }
        public virtual ICollection<Car> Cars { get; set; }
    }
}