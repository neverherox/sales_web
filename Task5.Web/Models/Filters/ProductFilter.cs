using System.ComponentModel.DataAnnotations;

namespace Task5.Web.Models.Filters
{
    public class ProductFilter : BaseFilter
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Цена")]
        public double Price { get; set; }
    }
}