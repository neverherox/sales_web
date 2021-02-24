using System.ComponentModel.DataAnnotations;

namespace Task5.Web.Models.Product
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Цена")]
        public double Price { get; set; }
    }
}