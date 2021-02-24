using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Task5.Web.Models.Order
{
    public class CreateOrderViewModel
    {
        [Required]
        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Требуется поле Продукт.")]
        [Display(Name = "Продукт")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Требуется поле Клиент.")]
        [Display(Name = "Клиент")]
        public int ClientId { get; set; }

        public SelectList Clients;

        public SelectList Products;
    }
}