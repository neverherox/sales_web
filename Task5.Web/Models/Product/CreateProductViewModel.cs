using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Task5.Web.Models.Product
{
    public class CreateProductViewModel
    {
        [Required]
        [Remote("CheckProductName", "Product", ErrorMessage = "Продукт с таким названием уже существует")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Длина строки должна быть от 2 до 30 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Цена")]
        [Min(1, ErrorMessage = "Минимальная цена = 1")]
        public double Price { get; set; }
    }
}