using System.Collections.Generic;

namespace Task5.DAL.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public Client()
        {
            Orders = new List<Order>();
        }
    }
}
