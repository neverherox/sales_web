using System.ComponentModel.DataAnnotations;

namespace Task5.Web.Models.Client
{
    public class ClientViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }
    }
}