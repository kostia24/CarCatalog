using System.Data.Entity;
using System.Web;
using CarsCatalog.Controllers;
using CarsCatalog.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;

namespace CarsCatalog.Models
{
    public static class ModelContainer
    {
        private static readonly object Key = new object();
        private static UnityContainer _instance;

        public static UnityContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Key)
                    {
                        if (_instance != null) return _instance;
                        _instance = new UnityContainer();
                        _instance.RegisterType<IBrandCarTreeRepository, BrandCarRepository>();
                        _instance.RegisterType<IBrandRepository, BrandCarRepository>();
                        _instance.RegisterType<IModelCarRepository, ModelCarRepository>();
                        _instance.RegisterType<ICarFiltersRepository, CarRepository>();
                        _instance.RegisterType<ICarRepository, CarRepository>();
                        _instance.RegisterType<IPriceRepository, ChangePriceRepository>();

                        _instance.RegisterType<UserManager<ApplicationUser>>(new HierarchicalLifetimeManager());
                        _instance.RegisterType<IAuthenticationManager>(new InjectionFactory(o => HttpContext.Current.GetOwinContext().Authentication));
                        _instance.RegisterType<DbContext, CatalogDbContext>(new HierarchicalLifetimeManager());
                        _instance.RegisterType<AccountController>(new InjectionConstructor());
                        _instance.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new InjectionConstructor(new CatalogDbContext()));

                    }
                }
                    return _instance;
            }
        }
    }
}