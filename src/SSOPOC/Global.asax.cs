﻿using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSOPOC
{
    public class EPiServerApplication : EPiServer.Global
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //Tip: Want to call the EPiServer API on startup? Add an initialization module instead (Add -> New Item.. -> EPiServer -> Initialization Module)
        }

        protected override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);
            routes.MapRoute("Custom login", "Login/{action}/", new { controller = "Login", action = "Index" });
            routes.MapRoute("PingOne", "PingOne/{action}/", new { controller = "PingOne", action = "Index" });
        }

    }
}
