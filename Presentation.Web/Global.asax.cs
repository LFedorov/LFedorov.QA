using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Domain.Common.IoC;

namespace Presentation.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            IoCConfig.RegisterDependencies();
        }
    }

    public class WebResolver : IResolver
    {
        public T Resolve<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }

        public object Resolve(Type type)
        {
            return DependencyResolver.Current.GetService(type);
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            return DependencyResolver.Current.GetServices(type);
        }
    }
}
