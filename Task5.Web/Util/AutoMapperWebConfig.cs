using AutoMapper;
using Task5.BL.DTO;
using Task5.Web.Models;
using Task5.Web.Models.Client;
using Task5.Web.Models.Order;
using Task5.Web.Models.Product;

namespace Task5.Web.Util
{
    public class AutoMapperWebConfig
    {
        public static MapperConfiguration Configure()
        {
            var config = new MapperConfiguration
            (
                cfg =>
                {
                    cfg.CreateMap<ProductDTO, ProductViewModel>().ReverseMap();
                    cfg.CreateMap<ClientViewModel, ClientDTO>().ReverseMap();
                    cfg.CreateMap<OrderDTO, OrderViewModel>();
                    cfg.CreateMap<CreateOrderViewModel, OrderDTO>();
                    cfg.CreateMap<CreateClientViewModel, ClientDTO>();
                    cfg.CreateMap<CreateProductViewModel, ProductDTO>();

                }
            );
            return config;
        }
    }
}