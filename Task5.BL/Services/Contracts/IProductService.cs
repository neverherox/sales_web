using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Task5.BL.DTO;

namespace Task5.BL.Services.Contracts
{
    public interface IProductService
    {
        ProductDTO GetProduct(Expression<Func<ProductDTO, bool>> predicate);
        IEnumerable<ProductDTO> GetProducts();
        void Create(ProductDTO productDTO);
        void Update(ProductDTO productDTO);
        void Remove(ProductDTO productDTO);
    }
}
