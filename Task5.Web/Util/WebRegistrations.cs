using Ninject.Modules;
using Task5.BL.Services;
using Task5.BL.Services.Contracts;

namespace Task5.Web.Util
{
    public class WebRegistrations : NinjectModule
    {
        public override void Load()
        {
            Bind<IOrderService>().To<OrderService>();
            Bind<IClientService>().To<ClientService>();
            Bind<IProductService>().To<ProductService>();
        }
    }
}
