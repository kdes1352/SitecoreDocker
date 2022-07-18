using System.Linq;
using System.Web;
using System.Xml.Linq;
using AllinaHealth.Framework.ContentSearch;
using AllinaHealth.Framework.Contexts;
using AllinaHealth.Framework.Links;
using AllinaHealth.Models.ContentSearch;
using Sitecore;
using Sitecore.Caching;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Links;
using Sitecore.Mvc.Extensions;
using Sitecore.Pipelines.HttpRequest;

namespace AllinaHealth.Framework.Pipelines.HttpRequestBegin
{
    public class SitemapRequestProcessor : HttpRequestProcessor
    {
        private const string SitemapFileName = "allinahealth_sitemap.xml";

        public override void Process(HttpRequestArgs args)
        {
            var context = HttpContext.Current;
            if (context == null)
            {
                return;
            }

            var requestUrl = context.Request.Url.ToString().ToLower();
            if (string.IsNullOrEmpty(requestUrl) || !requestUrl.EndsWith(SitemapFileName))
            {
                return;
            }

            var sitemapFromCache = string.Empty;
            var htmlCache = Context.Site.ValueOrDefault(CacheManager.GetHtmlCache);
            if (htmlCache != null)
            {
                sitemapFromCache = htmlCache.GetHtml(Context.Site.Name + "_" + SitemapFileName);
            }

            if (string.IsNullOrEmpty(sitemapFromCache))
            {
                var homeItem = SiteContext.Current.HomeItem;
                var predicate = PredicateBuilder.True<SitemapSearchResultItem>();
                //predicate = predicate.And(e => e.Language == Sitecore.Context.Language.Name);
                predicate = predicate.And(e => e.IncludeInSitemap == true);
                predicate = predicate.And(e => e.Path.StartsWith(homeItem.Paths.FullPath));

                var results = IndexSearcher.Search(predicate, e => e.Path);



                var options = LinkManager.GetDefaultUrlOptions();
                options.AlwaysIncludeServerUrl = true;
                XNamespace ns = @"http://www.sitemaps.org/schemas/sitemap/0.9";
                var xmlSitemap = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement(ns + "urlset",
                        results.Select(page => new XElement(ns + "url",
                            new XElement(ns + "loc", LinkHelper.RemoveSslPort(LinkManager.GetItemUrl(page.Document.GetItem(), options))),
                            new XElement(ns + "lastmod", page.Document.Updated.ToString("yyyy-MM-dd"))))));

                sitemapFromCache = string.Concat(xmlSitemap.Declaration.ToString(), xmlSitemap.ToString());
                htmlCache?.SetHtml(Context.Site.Name + "_" + SitemapFileName, sitemapFromCache);
            }

            context.Response.ContentType = "text/xml";
            context.Response.Write(sitemapFromCache);
            context.Response.End();
        }
    }
}