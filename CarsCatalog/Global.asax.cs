using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CarsCatalog.Helpers;
using CarsCatalog.Models;
using ModelContainer = CarsCatalog.Models.ModelContainer;

namespace CarsCatalog
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer(new CatalogDbInitializer()); // uncomment to initialize car catalog with generated data

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DependencyResolver.SetResolver(new UnityDependencyResolver(ModelContainer.Instance));
        }
    }
}
