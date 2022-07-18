using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using AllinaHealth.Models.Extensions;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Resources.Media;

namespace AllinaHealth.Framework.Pipelines.HttpRequestBegin
{
    public class DmsAndPdfDocumentResolver : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (Sitecore.Context.Item != null || Sitecore.Context.Database == null)
            {
                return;
            }

            var filesFolder = Sitecore.Context.Database.GetItem("/sitecore/media library/Allina Health/files");
            if (filesFolder.IsNull())
            {
                return;
            }

            var currentItem = filesFolder;
            var path = args.RequestUrl.AbsolutePath.ToLower();
            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal));
            }

            var pathArray = GetDocumentPath(path);
            foreach (var p in pathArray.TakeWhile(p => !currentItem.IsNull()))
            {
                currentItem = currentItem.GetChildrenSafe().FirstOrDefault(e => e.Name == p);
            }

            if (currentItem.IsNull())
            {
                return;
            }

            //301 redirect
            var mediaItem = (MediaItem)currentItem;
            var mediaUrl = MediaManager.GetMediaUrl(mediaItem);
            var redirectToUrl = Sitecore.StringUtil.EnsurePrefix('/', mediaUrl);

            HttpContext.Current.Response.Status = "301 Moved Permanently";
            HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.MovedPermanently;
            HttpContext.Current.Response.AddHeader("Location", redirectToUrl);
            HttpContext.Current.Response.End();
        }

        private static IEnumerable<string> GetDocumentPath(string rawUrl)
        {
            rawUrl = HttpUtility.UrlDecode(rawUrl.ToLower());
            rawUrl = rawUrl.Replace("/uploadedfiles/content", string.Empty).Replace("_", " ").Replace("-", " ").Replace("   ", " ").Replace("  ", " ").Replace("(", string.Empty).Replace(")", string.Empty);
            var lastPeriod = rawUrl.LastIndexOf(".", StringComparison.Ordinal);
            if (lastPeriod > 0)
            {
                rawUrl = rawUrl.Substring(0, lastPeriod);
            }

            if (!rawUrl.StartsWith("/"))
            {
                rawUrl = "/" + rawUrl;
            }

            if (rawUrl.StartsWith("/pages/"))
            {
                rawUrl = rawUrl.Replace("/pages/", "/");
            }

            var pathArray = rawUrl.Split('/');
            return (from p in pathArray where !string.IsNullOrEmpty(p) select ItemUtil.ProposeValidItemName(p.Trim())).ToList();
        }
    }
}