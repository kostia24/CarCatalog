using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CarsCatalog.Models
{
    public class Car
    {
        [Required]
        public int Id{ get; set; }
        [Required]
        [StringLength(20, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-z]{3,10}$",ErrorMessage = "Wrong color format")]
        [Remote("CheckColor", "Cars", ErrorMessage = "Color is undefined")]
        public string Color { get; set; }

        [Range(100, 10000000, ErrorMessage = "Out of price range")]
        public int Price{ get; set; }

        [Required]
        [Display(Name = "Engine capacity")]
        [Range(0.2, 50.0, ErrorMessage = "Out of engine capacity range")]
        [RegularExpression(@"^\d{1,2}(\.\d{1,3})?$", ErrorMessage = "Wrong engine capacity format. Example: 1.8")]
        public string EngineCapacity { get; set; }
        public string Description{ get; set; }

        public byte[] ImageData { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string ImageMimeType { get; set; }

        public int ModelId{ get; set; }
        public virtual ICollection<PriceChangeHistory> PriceChangeHistories { get; set; } 

        public virtual CarModel Model{ get; set; }
    }
}