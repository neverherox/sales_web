using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Task5.Web.Models.Client
{
    public class CreateClientViewModel
    {

        [Required]
        [Display(Name = "Имя")]
        [Remote("CheckClientName", "Client", ErrorMessage = "Клиент с таким именем уже существует")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Длина строки должна быть от 2 до 30 символов")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Номер телефона")]
        [Remote("CheckClientPhoneNumber", "Client", ErrorMessage = "Клиент с таким номером телефона уже существует")]
        [RegularExpression(@"^(\+375|80)(29|25|44|33)(\d{3})(\d{2})(\d{2})$", ErrorMessage = "Некорректный формат номера (+375|80)(29|25|44|33)(6 цифр)")]
        public string PhoneNumber { get; set; }
    }
}