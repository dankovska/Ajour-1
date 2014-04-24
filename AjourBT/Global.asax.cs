using AjourBT.Domain.Concrete;
using AjourBT.Filters;
using AjourBT.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AjourBT
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string Culture;
        public static string DatePattern;
        public static string JSDatePattern;
        
        protected void Application_Start()
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(@"/");
            GlobalizationSection section =
              (GlobalizationSection)config.GetSection("system.web/globalization");
            Culture = section.Culture.ToString();

            DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture(Culture).DateTimeFormat;

            DatePattern = dtfi.ShortDatePattern;
            JSDatePattern = DatePattern.Replace("M", "m").Replace("yy", "y");
            
                              

            AreaRegistration.RegisterAllAreas();

            DependencyResolver.SetResolver(new NinjectDependencyResolver()); 

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            GlobalFilters.Filters.Add(new DisableCache());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            //Database.SetInitializer<AjourDbContext>(new AjourDbInitializer());
            // DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(AjourBT.Domain.Entities.Permit.RequiredIfAttribute),typeof(RequiredAttributeAdapter));
        }

    }
}