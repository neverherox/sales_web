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
        private IGenericRepository<Client> clientRepository;
        private IGenericRepository<Order> orderRepository;
        private IGenericRepository<Product> productRepository;
        private SalesContext salesContext;
        private bool disposed = false;

        public IGenericRepository<Client> ClientRepository => clientRepository;
        public IGenericRepository<Order> OrderRepository => orderRepository;
        public IGenericRepository<Product> ProductRepository => productRepository;

        public SalesUnitOfWork()
        {
            salesContext = new SalesContext();
            salesContext.Database.CreateIfNotExists();
            clientRepository = new GenericRepository<Client>(salesContext);
            orderRepository = new GenericRepository<Order>(salesContext);
            productRepository = new GenericRepository<Product>(salesContext);
        }
        public void SaveContext()
        {
            salesContext.SaveChanges();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    salesContext.Dispose();
                }
            }
            disposed = true;
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
