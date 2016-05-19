using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AzureStorageHelperLibrary;

namespace AzureStorageHelperExample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
                        
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            //AzureStorageHelper.Helpers.AzureStorageHelper.CreateOrGetContainer("filestorage");
            AzureStorageHelper.Initialize(
                ConfigurationManager.AppSettings["BlobStorageAccountPath"],
                ConfigurationManager.AppSettings["BlobStorageAccountName"],
                ConfigurationManager.AppSettings["BlobStorageAccountAuthKey"],
                ConfigurationManager.AppSettings["BlobStorageContainer"]);
        }
    }
}
