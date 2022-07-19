using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Pipelines;

namespace AllinaHealth.Web
{
    public class InitializeRoutes
    {
        public void Process(PipelineArgs args)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "SearchIndex",
                url: "searchindex/{language}/{database}/{itemId}",
                defaults: new { controller = "Search", action = "GetHtmlForIndex" }
            );

            routes.MapRoute(
                name: "Sitemap_xml",
                url: "sitemap.xml",
                defaults: new { controller = "Sitemap", action = "Index" }
            );

            routes.MapRoute(
                name: "CareersLookup",
                url: "ah/{controller}/{action}",
                defaults: new { controller = "Careers", action = "LookupJobs" }
            );

            routes.MapRoute(
                name: "SEO_Reset_URL_Fields",
                url: "ah/{controller}/{action}",
                defaults: new { controller = "NewsBlog", action = "Reset_SEO_URLs" }
            );

            //routes.MapRoute(
            //    name: "NavigateToCareersSearchResults",
            //    url: "ah/{controller}/{action}",
            //    defaults: new { controller = "Careers", action = "NavigateToCareersSearchResults" }
            //);

            routes.MapRoute(
                name: "Default",
                url: "ah/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}