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
    public class ProductService : IProductService
    {
        private ISalesUnitOfWork uow;
        private IMapper mapper;
        private bool disposed = false;
        public ProductService(ISalesUnitOfWork uow)
        {
            this.uow = uow;
            mapper = new Mapper(AutoMapperBLConfig.Configure());
        }
        public ProductDTO GetProduct(Expression<Func<ProductDTO, bool>> predicate)
        {
            var newPredicate = mapper.Map<Expression<Func<ProductDTO, bool>>, Expression<Func<Product, bool>>>(predicate);
            var product = mapper.Map<Product, ProductDTO>(uow.ProductRepository.Get(newPredicate));
            return product;
        }
        public IEnumerable<ProductDTO> GetProducts()
        {
            return uow.ProductRepository.Get().ProjectTo<ProductDTO>(AutoMapperBLConfig.Configure()).ToList();
        }
        public void Create(ProductDTO productDTO)
        {
            var product = mapper.Map<ProductDTO, Product>(productDTO);
            uow.ProductRepository.Add(product);
            uow.SaveContext();
        }
        public void Update(ProductDTO productDTO)
        {
            var product = uow.ProductRepository.Get(x => x.Id == productDTO.Id);
            product.Name = productDTO.Name;
            product.Price = productDTO.Price;
            uow.ProductRepository.Update(product);
            uow.SaveContext();
        }
        public void Remove(ProductDTO productDTO)
        {
            var product = uow.ProductRepository.Get(x => x.Id == productDTO.Id);
            uow.ProductRepository.Delete(product);
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

        ~ProductService()
        {
            Dispose(false);
        }
    }
}
