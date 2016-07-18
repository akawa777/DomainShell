﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DomainShell.Tests.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Api",
                url: "api/{controller}/{action}/{id}",
                defaults: new { controller = "Person", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "App",
                url: "{app}/{main}/{id}",
                defaults: new 
                    { 
                        controller = "Facade", 
                        action = "Index", 
                        app = "Person",
                        main = "List", 
                        id = UrlParameter.Optional 
                    }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Person", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
