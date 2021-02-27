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
        private ISalesUnitOfWork _uow;
        private IMapper _mapper;
        private bool _disposed = false;
        public ClientService(ISalesUnitOfWork uow)
        {
            _uow = uow;
            _mapper = new Mapper(AutoMapperBLConfig.Configure());
        }
        public ClientDTO GetClient(Expression<Func<ClientDTO, bool>> predicate)
        {
            var newPredicate = _mapper.Map<Expression<Func<ClientDTO, bool>>, Expression<Func<Client, bool>>>(predicate);
            var client = _mapper.Map<Client, ClientDTO>(_uow.ClientRepository.Get(newPredicate));
            return client;
        }
        public IEnumerable<ClientDTO> GetClients()
        {
            return _uow.ClientRepository.Get().ProjectTo<ClientDTO>(AutoMapperBLConfig.Configure()).ToList();
        }

        public void Create(ClientDTO clientDTO)
        {
            var client = _mapper.Map<ClientDTO, Client>(clientDTO);
            _uow.ClientRepository.Add(client);
            _uow.SaveContext();
        }
        public void Remove(ClientDTO clientDTO)
        {
            var client = _uow.ClientRepository.Get(x => x.Id == clientDTO.Id);
            _uow.ClientRepository.Delete(client);
            _uow.SaveContext();
        }
        public void Update(ClientDTO clientDTO)
        {
            var client = _uow.ClientRepository.Get(x => x.Id == clientDTO.Id);
            client.Name = clientDTO.Name;
            client.PhoneNumber = clientDTO.PhoneNumber;
            _uow.ClientRepository.Update(client);
            _uow.SaveContext();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _uow.Dispose();
                }
            }
            _disposed = true;
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
