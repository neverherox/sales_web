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
    public class ClientService : IClientService
    {
        private ISalesUnitOfWork uow;
        private IMapper mapper;
        private bool disposed = false;
        public ClientService(ISalesUnitOfWork uow)
        {
            this.uow = uow;
            mapper = new Mapper(AutoMapperBLConfig.Configure());
        }
        public ClientDTO GetClient(Expression<Func<ClientDTO, bool>> predicate)
        {
            var newPredicate = mapper.Map<Expression<Func<ClientDTO, bool>>, Expression<Func<Client, bool>>>(predicate);
            var client = mapper.Map<Client, ClientDTO>(uow.ClientRepository.Get(newPredicate));
            return client;
        }
        public IEnumerable<ClientDTO> GetClients()
        {
            return uow.ClientRepository.Get().ProjectTo<ClientDTO>(AutoMapperBLConfig.Configure()).ToList();
        }

        public void Create(ClientDTO clientDTO)
        {
            var client = mapper.Map<ClientDTO, Client>(clientDTO);
            uow.ClientRepository.Add(client);
            uow.SaveContext();
        }
        public void Remove(ClientDTO clientDTO)
        {
            var client = uow.ClientRepository.Get(x => x.Id == clientDTO.Id);
            uow.ClientRepository.Delete(client);
            uow.SaveContext();
        }
        public void Update(ClientDTO clientDTO)
        {
            var client = uow.ClientRepository.Get(x => x.Id == clientDTO.Id);
            client.Name = clientDTO.Name;
            uow.ClientRepository.Update(client);
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

        ~ClientService()
        {
            Dispose(false);
        }
    }
}
