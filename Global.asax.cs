using CQRS.Common;
using CQRS.ES;
using SimpleCQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SyncApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            var ebus = new EventBus();            
      
            var detail = new InventoryItemDetailView();
            ebus.RegisterHandler<InventoryItemCreated>(detail.Handle);
            ebus.RegisterHandler<InventoryItemDeactivated>(detail.Handle);
            ebus.RegisterHandler<InventoryItemRenamed>(detail.Handle);
            ebus.RegisterHandler<ItemsCheckedInToInventory>(detail.Handle);
            ebus.RegisterHandler<ItemsRemovedFromInventory>(detail.Handle);
            var list = new InventoryListView();
            ebus.RegisterHandler<InventoryItemCreated>(list.Handle);
            ebus.RegisterHandler<InventoryItemRenamed>(list.Handle);
            ebus.RegisterHandler<InventoryItemDeactivated>(list.Handle);
            ServiceLocator.Bus = ebus;
        }
    }
}
