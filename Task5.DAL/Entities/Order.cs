using System;

namespace Task5.DAL.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public virtual Client Client { get; set; }
        public virtual Product Product { get; set; }
        public int ProductId { get; set; }
        public int ClientId { get; set; }
    }
}
