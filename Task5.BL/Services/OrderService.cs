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
        private ISalesUnitOfWork _uow;
        private IMapper _mapper;
        private bool _disposed = false;
        public OrderService(ISalesUnitOfWork uow)
        {
            _uow = uow;
            _mapper = new Mapper(AutoMapperBLConfig.Configure());
        }
        public IEnumerable<OrderDTO> GetOrders()
        {
            return _uow.OrderRepository.Get().ProjectTo<OrderDTO>(AutoMapperBLConfig.Configure()).ToList();
        }
        public IEnumerable<OrderDTO> GetOrders(Expression<Func<OrderDTO, bool>> predicate)
        {
            var newPredicate = _mapper.Map<Expression<Func<OrderDTO, bool>>, Expression<Func<Order, bool>>>(predicate);
            return _uow.OrderRepository.Get()
                                      .Where(newPredicate)
                                      .ProjectTo<OrderDTO>(AutoMapperBLConfig.Configure()).ToList();
            
        }
        public OrderDTO GetOrder(Expression<Func<OrderDTO, bool>> predicate)
        {
            var newPredicate = _mapper.Map<Expression<Func<OrderDTO, bool>>, Expression<Func<Order, bool>>>(predicate);
            var orderDTO = _mapper.Map<Order, OrderDTO>(_uow.OrderRepository.Get(newPredicate));
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
            _uow.OrderRepository.Add(order);
            _uow.SaveContext();
        }
        public void Update(OrderDTO orderDTO)
        {
            var order = _uow.OrderRepository.Get(x => x.Id == orderDTO.Id);
            order.ClientId = orderDTO.ClientId;
            order.ProductId = orderDTO.ProductId;
            order.Date = orderDTO.Date;
            _uow.OrderRepository.Update(order);
            _uow.SaveContext();
        }

        public void Remove(OrderDTO orderDTO)
        {
            var order = _uow.OrderRepository.Get(x => x.Id == orderDTO.Id);
            _uow.OrderRepository.Delete(order);
            _uow.SaveContext();
        }
    }
}
