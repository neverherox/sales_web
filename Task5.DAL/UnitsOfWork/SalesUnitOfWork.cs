using System;
using Task5.DAL.Context;
using Task5.DAL.Entities;
using Task5.DAL.Repositories;
using Task5.DAL.Repositories.Contracts;
using Task5.DAL.UnitsOfWork.Contracts;

namespace Task5.DAL.UnitsOfWork
{
    public class SalesUnitOfWork : ISalesUnitOfWork
    {
        private IGenericRepository<Client> _clientRepository;
        private IGenericRepository<Order> _orderRepository;
        private IGenericRepository<Product> _productRepository;
        private SalesContext _salesContext;
        private bool _disposed = false;

        public IGenericRepository<Client> ClientRepository => _clientRepository;
        public IGenericRepository<Order> OrderRepository => _orderRepository;
        public IGenericRepository<Product> ProductRepository => _productRepository;

        public SalesUnitOfWork()
        {
            _salesContext = new SalesContext();
            _salesContext.Database.CreateIfNotExists();
            _clientRepository = new GenericRepository<Client>(_salesContext);
            _orderRepository = new GenericRepository<Order>(_salesContext);
            _productRepository = new GenericRepository<Product>(_salesContext);
        }
        public void SaveContext()
        {
            _salesContext.SaveChanges();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _salesContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SalesUnitOfWork()
        {
            Dispose(false);
        }
    }
}
