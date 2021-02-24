using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Task5.BL.DTO;

namespace Task5.BL.Services.Contracts
{
    public interface IOrderService : IDisposable
    {
        IEnumerable<OrderDTO> GetOrders();
        IEnumerable<OrderDTO> GetOrders(Expression<Func<OrderDTO, bool>> predicate);
        OrderDTO GetOrder(Expression<Func<OrderDTO, bool>> predicate);
        void Update(OrderDTO orderDTO);
        void Create(OrderDTO orderDTO);
        void Remove(OrderDTO orderDTO);
    }
}
