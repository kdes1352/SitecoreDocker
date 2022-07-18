using System.IO;
using System.Net;
using System.Web;
using AllinaHealth.Framework.Contexts;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;

namespace AllinaHealth.Framework.Pipelines.HttpRequestBegin
{
    public class NotFoundItemResolver : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (Sitecore.Context.Item != null || Sitecore.Context.Database == null)
            {
                return;
            }

            if (Sitecore.Context.Site.Name.ToLower() != "website")
            {
                return;
            }

            var filePath = args.Url.FilePath.ToLower();
            if (filePath.StartsWith("/sitecore")
                || filePath.StartsWith("/-/")
                || filePath.StartsWith("/ah/")
                || filePath.StartsWith("/api/")
                || filePath.StartsWith("/searchindex/")
                || filePath.StartsWith("/assets/")
                || filePath.StartsWith("/formbuilder/"))
            {
                Sitecore.Context.Item = Sitecore.Context.Database.GetItem(Sitecore.ItemIDs.RootID);
                return;
            }

            if (!string.IsNullOrEmpty(args.HttpContext.Request.CurrentExecutionFilePathExtension) || File.Exists(args.HttpContext.Request.PhysicalPath))
            {
                return;
            }

            var notFoundPage = SiteContext.Current.Page404;
            if (notFoundPage == null)
            {
                return;
            }

            Sitecore.Context.Item = notFoundPage;
            HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;
            HttpContext.Current.Response.StatusDescription = "Page not found";
            HttpContext.Current.Response.TrySkipIisCustomErrors = true;
        }
    }
}