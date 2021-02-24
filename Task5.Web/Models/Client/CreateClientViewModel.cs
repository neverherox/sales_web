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
    }
}