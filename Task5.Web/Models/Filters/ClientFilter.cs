using System.ComponentModel.DataAnnotations;

namespace Task5.Web.Models.Filters
{
    public class ClientFilter : BaseFilter
    {
        [Display(Name = "Имя")]
        public string Name { get; set; }
    }
}