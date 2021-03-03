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
        private ISalesUnitOfWork _uow;
        private IMapper _mapper;
        private bool _disposed = false;
        public ProductService(ISalesUnitOfWork uow)
        {
            _uow = uow;
            _mapper = new Mapper(AutoMapperBLConfig.Configure());
        }
        public ProductDTO GetProduct(Expression<Func<ProductDTO, bool>> predicate)
        {
            var newPredicate = _mapper.Map<Expression<Func<ProductDTO, bool>>, Expression<Func<Product, bool>>>(predicate);
            var product = _mapper.Map<Product, ProductDTO>(_uow.ProductRepository.Get(newPredicate));
            return product;
        }
        public IEnumerable<ProductDTO> GetProducts()
        {
            return _uow.ProductRepository.Get().ProjectTo<ProductDTO>(AutoMapperBLConfig.Configure()).ToList();
        }
        public void Create(ProductDTO productDTO)
        {
            var product = _mapper.Map<ProductDTO, Product>(productDTO);
            _uow.ProductRepository.Add(product);
            _uow.SaveContext();
        }
        public void Update(ProductDTO productDTO)
        {
            var product = _uow.ProductRepository.Get(x => x.Id == productDTO.Id);
            product.Name = productDTO.Name;
            product.Price = productDTO.Price;
            _uow.ProductRepository.Update(product);
            _uow.SaveContext();
        }
        public void Remove(ProductDTO productDTO)
        {
            var product = _uow.ProductRepository.Get(x => x.Id == productDTO.Id);
            _uow.ProductRepository.Delete(product);
            _uow.SaveContext();
        }
    }
}
