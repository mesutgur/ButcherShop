using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ButcherShop.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();

            if (exception != null)
            {
                // In production, log to a file or logging service
                // For now, we'll just clear the error

                // Log the exception details
                var errorMessage = $"Error: {exception.Message}\nStack Trace: {exception.StackTrace}";

                // You can implement file logging here or use a logging framework like NLog/Serilog
                // Example: Logger.Error(exception, errorMessage);

                Server.ClearError();

                // Redirect to error page
                Response.Redirect("~/Error");
            }
        }
    }
}
