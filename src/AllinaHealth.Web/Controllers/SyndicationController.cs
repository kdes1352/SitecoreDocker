using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using AllinaHealth.Models.Extensions;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;

namespace AllinaHealth.Web.Controllers
{
    public class SyndicationController : Controller
    {
        public ActionResult FeedList()
        {
            return View("~/Views/Containers/Lists/FeedList.cshtml", GetSyndicationItems(Sitecore.Mvc.Presentation.RenderingContext.Current.Rendering.Item));
        }

        private List<SyndicationItem> GetSyndicationItems(Item i)
        {
            try
            {
                var url = "";
                //If RSS Feed is from Sitecore, need the full Sitecore URL (including server) - Cannot use Item.GetLinkFieldUrl() extension
                var lf = (LinkField)i.Fields["Syndication Item"];
                if (lf != null)
                {
                    if (lf.TargetItem != null)
                    {
                        var options = LinkManager.GetDefaultUrlOptions();
                        options.AlwaysIncludeServerUrl = true;
                        url = LinkManager.GetItemUrl(lf.TargetItem, options);
                    }
                    else
                    {
                        url = lf.Url;
                    }
                }

                if (string.IsNullOrEmpty(url))
                {
                    url = i.GetFieldValue("Syndication Url");
                }

                var take = i.GetFieldInteger("Max Number of Articles", int.MaxValue);
                var syndicationItems = new List<SyndicationItem>();
                syndicationItems.AddRange(GetItemsFromUrl(url));
                return syndicationItems.OrderByDescending(e => e.PublishDate).Take(take).ToList();
            }
            catch (Exception e)
            {
                Log.Error("SyndicationModel.GetSyndicationItems from URL: " + System.Web.HttpContext.Current.Request.RawUrl, e, this);
            }

            return new List<SyndicationItem>();
        }

        private static IEnumerable<SyndicationItem> GetItemsFromUrl(string url)
        {
            var list = new List<SyndicationItem>();
            using (var reader = XmlReader.Create(url, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse }))
            {
                var feed = SyndicationFeed.Load(reader);

                foreach (var item in feed.Items)
                {
                    item.Copyright = (feed.Copyright == null) ? new TextSyndicationContent(feed.Generator) : new TextSyndicationContent(feed.Copyright.Text);
                    if (item.Links.Count > 0)
                    {
                        item.Id = item.Links[0].Uri.ToString();
                    }

                    list.Add(item);
                }
            }

            return list;
        }
    }
}