using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Task5.BL.DTO;

namespace Task5.BL.Services.Contracts
{
    public interface IClientService
    {
        ClientDTO GetClient(Expression<Func<ClientDTO, bool>> predicate);
        IEnumerable<ClientDTO> GetClients();
        void Create(ClientDTO clientDTO);
        void Remove(ClientDTO clientDTO);
        void Update(ClientDTO clientDTO);
    }
}
