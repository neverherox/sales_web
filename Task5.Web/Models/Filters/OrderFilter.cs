using System;
using System.ComponentModel.DataAnnotations;

namespace Task5.Web.Models.Filters
{
    public class OrderFilter : BaseFilter
    {
        [Display(Name = "Клиент")]
        public string ClientName { get; set; }

        [Display(Name = "Продукт")]
        public string ProductName { get; set; }

        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
    }
}