using System;

namespace Task5.DAL.UnitsOfWork.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveContext();
    }
}
