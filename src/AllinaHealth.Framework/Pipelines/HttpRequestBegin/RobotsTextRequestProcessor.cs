using System;
using System.Web;
using AllinaHealth.Framework.Contexts;
using AllinaHealth.Models.Extensions;
using Sitecore.Pipelines.HttpRequest;

namespace AllinaHealth.Framework.Pipelines.HttpRequestBegin
{
    public class RobotsTextRequestProcessor : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            var context = HttpContext.Current;

            if (context == null)
            {
                return;
            }

            var requestUrl = context.Request.Url.ToString();

            if (string.IsNullOrEmpty(requestUrl) || !requestUrl.ToLower().EndsWith("robots.txt"))
            {
                return;
            }

            var robotsTxtContent = @"User-agent: *"
                                   + Environment.NewLine +
                                   "Disallow: /";


            var homeItem = SiteContext.Current.HomeItem;
            if (homeItem != null)
            {
                var siteRobotsText = homeItem.GetFieldValue("Site Robots Text");
                if (!string.IsNullOrEmpty(siteRobotsText))
                {
                    robotsTxtContent = siteRobotsText;
                }
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(robotsTxtContent);
            context.Response.End();
        }
    }
}