using Autofac;
using Autofac.Integration.WebApi;
using LeapSpring.MJC.Infrastructure;
using Newtonsoft.Json.Converters;
using System.Reflection;
using System.Web.Http;

namespace LeapSpring.MJC.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter("Bearer"));

            // Enable CORS
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();
            

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            DependencyRegistrar.Register(builder, config);

            config.Formatters.JsonFormatter.SerializerSettings.Formatting =
               Newtonsoft.Json.Formatting.Indented;

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add
                (new StringEnumConverter());
        }
    }
}
