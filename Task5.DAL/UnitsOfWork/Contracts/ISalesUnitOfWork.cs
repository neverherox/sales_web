using Task5.DAL.Entities;
using Task5.DAL.Repositories.Contracts;

namespace Task5.DAL.UnitsOfWork.Contracts
{
    public interface ISalesUnitOfWork : IUnitOfWork
    {
        IGenericRepository<Client> ClientRepository { get; }
        IGenericRepository<Order> OrderRepository { get; }
        IGenericRepository<Product> ProductRepository { get; }
    }
}
