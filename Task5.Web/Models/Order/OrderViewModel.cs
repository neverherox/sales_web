using System;
using System.ComponentModel.DataAnnotations;

namespace Task5.Web.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Продукт")]
        public string ProductName { get; set; }

        [Display(Name = "Цена")]
        public double Price { get; set; }

        [Display(Name = "Клиент")]
        public string ClientName { get; set; }
    }
}