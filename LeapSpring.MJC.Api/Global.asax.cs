using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.Data;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace LeapSpring.MJC.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalConfiguration.Configuration.Filters.Add(new UnhandledExceptionFilter());

            //Database.SetInitializer<MJCDbContext>(new CreateDatabaseIfNotExists<MJCDbContext>());
            Database.SetInitializer<MJCDbContext>(new MigrateDatabaseToLatestVersion<MJCDbContext, Configuration>());
        }
    }
}
