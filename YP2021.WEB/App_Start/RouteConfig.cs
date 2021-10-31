using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nero2021
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute("UyeGirisi", "uye-girisi",
            new RouteValueDictionary(new { controller = "UyeGirisi", action = "Index" }),
            new MvcRouteHandler());


            routes.MapRoute("Yeniİeti", "yeni-ileti",
            new RouteValueDictionary(new { controller = "Ileti", action = "Yeni" }),
            new MvcRouteHandler());

            routes.MapRoute("GonderilmisIletiler", "gonderilmis-iletiler",
            new RouteValueDictionary(new { controller = "Ileti", action = "Gonderilmis" }),
            new MvcRouteHandler());

            routes.MapRoute("HazirSablonlar", "hazir-sablonlar",
            new RouteValueDictionary(new { controller = "IletiSablon", action = "List" }),
            new MvcRouteHandler());

            routes.MapRoute("SifremiUnuttum", "sifremi-unuttum",
            new RouteValueDictionary(new { controller = "SifremiUnuttum", action = "Index" }),
            new MvcRouteHandler());

            //routes.MapRoute("Kepikindex", "kep-ik",
            // new RouteValueDictionary(new { controller = "Kepik", action = "Index" }),
            // new MvcRouteHandler());

            // routes.MapRoute("kepikpdfonizleme", "kep-ik/{*pdfonizleme}",
            //new RouteValueDictionary(new { controller = "Kepik", action = "PDFOnizleme" }),
            //new MvcRouteHandler());

            // routes.MapRoute("kepikpdfonizleme", "kep-ik/{*bordrogonderim}",
            //new RouteValueDictionary(new { controller = "Kepik", action = "BordroGonderim" }),
            //new MvcRouteHandler());



            //routes.MapRoute("UyeOl", "uye-ol",
            //new RouteValueDictionary(new { controller = "UyeOl", action = "Index" }),
            //new MvcRouteHandler());

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "TurkBelge.Controllers" }
            );
        }
    }
}
