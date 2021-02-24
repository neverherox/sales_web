using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Task5.BL.DTO;
using Task5.BL.Services.Contracts;
using Task5.BL.Util;
using Task5.DAL.Entities;
using Task5.DAL.UnitsOfWork.Contracts;

namespace Task5.BL.Services
{
    public class OrderService : IOrderService
    {
        private ISalesUnitOfWork uow;
        private IMapper mapper;
        private bool disposed = false;
        public OrderService(ISalesUnitOfWork uow)
        {
            this.uow = uow;
            mapper = new Mapper(AutoMapperBLConfig.Configure());
        }
        public IEnumerable<OrderDTO> GetOrders()
        {
            return uow.OrderRepository.Get().ProjectTo<OrderDTO>(AutoMapperBLConfig.Configure()).ToList();
        }
        public IEnumerable<OrderDTO> GetOrders(Expression<Func<OrderDTO, bool>> predicate)
        {
            var newPredicate = mapper.Map<Expression<Func<OrderDTO, bool>>, Expression<Func<Order, bool>>>(predicate);
            return uow.OrderRepository.Get()
                                      .Where(newPredicate)
                                      .ProjectTo<OrderDTO>(AutoMapperBLConfig.Configure()).ToList();
            
        }
        public OrderDTO GetOrder(Expression<Func<OrderDTO, bool>> predicate)
        {
            var newPredicate = mapper.Map<Expression<Func<OrderDTO, bool>>, Expression<Func<Order, bool>>>(predicate);
            var orderDTO = mapper.Map<Order, OrderDTO>(uow.OrderRepository.Get(newPredicate));
            return orderDTO;
        }
        
        public void Create(OrderDTO orderDTO)
        {
            var order = new Order
            {
                Date = orderDTO.Date,
                ClientId = orderDTO.ClientId,
                ProductId = orderDTO.ProductId
            };
            uow.OrderRepository.Add(order);
            uow.SaveContext();
        }
        public void Update(OrderDTO orderDTO)
        {
            var order = uow.OrderRepository.Get(x => x.Id == orderDTO.Id);
            order.ClientId = orderDTO.ClientId;
            order.ProductId = orderDTO.ProductId;
            order.Date = orderDTO.Date;
            uow.OrderRepository.Update(order);
            uow.SaveContext();
        }

        public void Remove(OrderDTO orderDTO)
        {
            var order = uow.OrderRepository.Get(x => x.Id == orderDTO.Id);
            uow.OrderRepository.Delete(order);
            uow.SaveContext();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    uow.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~OrderService()
        {
            Dispose(false);
        }
    }
}
