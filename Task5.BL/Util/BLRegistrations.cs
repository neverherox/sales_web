using Ninject.Modules;
using Ninject.Web.Common;
using Task5.DAL.UnitsOfWork;
using Task5.DAL.UnitsOfWork.Contracts;

namespace Task5.BL.Util
{
    public class BLRegistrations : NinjectModule
    {
        public override void Load()
        {
            Bind<ISalesUnitOfWork>().To<SalesUnitOfWork>().InRequestScope();
        }
    }
}
