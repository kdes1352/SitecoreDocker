using System;
using System.Net;
using AllinaHealth.Framework.Extensions;
using AllinaHealth.Models.Extensions;
using HtmlAgilityPack;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace AllinaHealth.Framework.ContentSearch.ComputedFields
{
    public class HtmlComputedField : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        public object ComputeFieldValue(IIndexable indexable)
        {
            Assert.ArgumentNotNull(indexable, "indexable");
            var url = string.Empty;
            try
            {
                Item item = indexable as SitecoreIndexableItem;
                if (item?.Visualization.Layout == null || !item.Paths.FullPath.StartsWith(Sitecore.Constants.ContentPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                using (var webClient = new WebClient())
                {
                    var siteContext = item.GetSiteContext();
                    url = string.Format("http://{0}/searchindex/{1}/{2}/{3}", siteContext.TargetHostName, item.Language.Name, item.Database.Name, item.ID.ToShortID());
                    var pageContent = webClient.DownloadString(url);
                    var htmlBytes = System.Text.Encoding.Default.GetBytes(pageContent);
                    var html = System.Text.Encoding.UTF8.GetString(htmlBytes);
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);

                    // Strip out all the html tags, so we can index just the text
                    var content = htmlDocument.GetAllInnerTexts();
                    return content;
                }
            }
            catch (WebException webExc)
            {
                Log.Warn(string.Format("HtmlComputedField - WebException - {0} ({1}): {2}", indexable.Id, url, webExc.Message), webExc, this);
            }
            catch (Exception exc)
            {
                Log.Error(string.Format("HtmlComputedField - Exception - {0}: {1}", indexable.Id, exc.Message), exc, this);
            }

            return null;
        }
    }
}