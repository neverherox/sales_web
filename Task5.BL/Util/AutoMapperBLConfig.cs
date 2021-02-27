using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Task5.BL.DTO;
using Task5.DAL.Entities;

namespace Task5.BL.Util
{
    public class AutoMapperBLConfig
    {
        public static MapperConfiguration Configure()
        {
            var config = new MapperConfiguration
            (
                cfg =>
                {
                    cfg.CreateMap<Order, OrderDTO>()
                    .ForMember("ClientName", opt => opt.MapFrom(x => x.Client.Name))
                    .ForMember("ProductName", opt => opt.MapFrom(x => x.Product.Name))
                    .ForMember("Price", opt => opt.MapFrom(x => x.Product.Price))
                    .ForMember("ProductId", opt => opt.MapFrom(x => x.Product.Id))
                    .ForMember("ClientId", opt => opt.MapFrom(x => x.Client.Id));

                    cfg.CreateMap<ClientDTO, Client>().ReverseMap();
                    cfg.CreateMap<ProductDTO, Product>().ReverseMap();
                    cfg.AddExpressionMapping();
                }
            );
            return config;
        }
    }
}
