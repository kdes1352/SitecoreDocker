using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using AllinaHealth.Framework.Extensions;
using AllinaHealth.Models.ContentSearch;
using AllinaHealth.Models.Extensions;
using HtmlAgilityPack;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.SecurityModel;
using Sitecore.Text;

namespace AllinaHealth.Framework.ContentSearch.GlobalSearch
{
    public class SearchWebPageRepository
    {
        private static readonly WebClient WebClient = new WebClient();
        private List<string> _urlList;
        private Database _db;
        private UrlOptions _urlOptions;

        private Dictionary<string, PreferredSearchItem> _preferredSearchItems;

        private UrlOptions Options
        {
            get
            {
                if (_urlOptions != null)
                {
                    return _urlOptions;
                }

                _urlOptions = LinkManager.GetDefaultUrlOptions();
                _urlOptions.AlwaysIncludeServerUrl = true;
                _urlOptions.SiteResolving = true;
                return _urlOptions;
            }
        }

        private Database Db => _db ?? (_db = Sitecore.Configuration.Factory.GetDatabase("web"));

        public List<string> UrlList => _urlList ?? (_urlList = GetListOfUrls());

        private static List<string> GetListOfUrls()
        {
            var list = new List<string>();
            foreach (var sitemapUrl in new ListString(Sitecore.Configuration.Settings.GetSetting("ContentSearch.SearchIndex.Sitemaps")))
            {
                list.AddRange(GetListOfUrls(sitemapUrl));
            }

            return list;
        }

        private static IEnumerable<string> GetListOfUrls(string sitemapUrl)
        {
            var xml = WebClient.DownloadString(sitemapUrl);
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var manager = new XmlNamespaceManager(xmlDoc.NameTable);
            if (xmlDoc.DocumentElement != null)
            {
                manager.AddNamespace("s", xmlDoc.DocumentElement.NamespaceURI);
            }

            return (from XmlNode node in xmlDoc.SelectNodes("/s:urlset/s:url/s:loc", manager) select node.InnerText).ToList();
        }

        public List<SearchWebPageItem> GetAllSearchPages()
        {
            var list = new List<SearchWebPageItem>();

            LoadPreferredSearchItems();
            Log.Warn(string.Format("GetAllSearchPages(), PreferredSearchItem Count:{0}", _preferredSearchItems.Count), this);

            using (new SecurityDisabler())
            {
                using (new EventDisabler())
                {
                    var mName = Environment.MachineName;
                    Log.Warn(string.Format("SEARCH INDEX: Starting sitecore rebuild on {0}", mName), this);
                    list.AddRange(GetFromSitecore());
                    Log.Warn(string.Format("SEARCH INDEX: Ending sitecore rebuild on {0}", mName), this);

                    Log.Warn(string.Format("SEARCH INDEX: Starting sitemap rebuild on {0}", mName), this);
                    list.AddRange(GetFromSitemap());
                    Log.Warn(string.Format("SEARCH INDEX: Ending sitemap rebuild on {0}", mName), this);
                }
            }

            return list;
        }

        public List<SearchWebPageItem> GetFromSitecore()
        {
            var home = Db.GetItem("/sitecore/content/allina health/home");
            var items = home.Axes.GetDescendants().Where(e => e.Visualization.Layout != null).ToList();
            items.Add(home);

            return items.Select(GetSearchPageFromSitecore).Where(sp => sp != null).ToList();
        }

        private IEnumerable<SearchWebPageItem> GetFromSitemap()
        {
            var searchExclusions = GetSearchExclusions();
            return (from url in UrlList where !url.ContainsAny(searchExclusions) select GetSearchPageFromUrl(url) into sp where sp != null select sp).ToList();
        }

        public SearchWebPageItem GetSearchPageFromSitecore(Item item)
        {
            try
            {
                var siteContext = item.GetSiteContext();
                var url = string.Format("http://{0}/searchindex/{1}/{2}/{3}", siteContext.TargetHostName, item.Language.Name, item.Database.Name, item.ID.ToShortID());
                var pageContent = WebClient.DownloadString(url);
                var htmlBytes = System.Text.Encoding.Default.GetBytes(pageContent);
                var html = System.Text.Encoding.UTF8.GetString(htmlBytes);

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                // Strip out all the html tags, so we can index just the text
                var content = htmlDocument.GetAllInnerTexts();
                Options.Site = siteContext;
                var pageUrl = LinkManager.GetItemUrl(item, Options);
                if (pageUrl.StartsWith(":"))
                {
                    pageUrl = pageUrl.Substring(1);
                }

                var pt = item.GetFieldValue("Browser Title");
                if (string.IsNullOrEmpty(pt))
                {
                    pt = item.GetFieldValue("Page Title");
                }

                if (string.IsNullOrEmpty(pt))
                {
                    pt = item.Name;
                }

                var metaDescription = item.GetFieldValue("Meta Description");

                //Check for Preferred Search Item entry
                //For Site Core items need to use the item fullpath and not URL
                var itemFullPath = item.Paths.FullPath;
                if (!string.IsNullOrEmpty(itemFullPath))
                {
                    itemFullPath = item.Paths.FullPath.ToLower();
                }

                if (!_preferredSearchItems.ContainsKey(itemFullPath))
                    return new SearchWebPageItem
                    {
                        Url = pageUrl,
                        PageTitle = pt,
                        Content = content,
                        SitecoreID = item.ID.ToString(),
                        SitecorePath = item.Paths.FullPath.ToLower(),
                        MetaDescription = metaDescription,
                        PreferredSearchItem = false,
                        PreferredKeywords = null,
                        OverrideTitle = null,
                        OverrideDescription = null
                    };
                var pSi = _preferredSearchItems[itemFullPath];
                const bool preferredSeachItem = true;
                var preferredKeywords = pSi.PreferredKeywords;
                var overrideTitle = pSi.OverrideTitle;
                var overrideDesc = pSi.OverrideDescription;

                return new SearchWebPageItem
                {
                    Url = pageUrl,
                    PageTitle = pt,
                    Content = content,
                    SitecoreID = item.ID.ToString(),
                    SitecorePath = item.Paths.FullPath.ToLower(),
                    MetaDescription = metaDescription,
                    PreferredSearchItem = preferredSeachItem,
                    PreferredKeywords = preferredKeywords,
                    OverrideTitle = overrideTitle,
                    OverrideDescription = overrideDesc
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public SearchWebPageItem GetSearchPageFromUrl(string url)
        {
            try
            {
                var pageContent = WebClient.DownloadString(url);
                var htmlBytes = System.Text.Encoding.Default.GetBytes(pageContent);
                var html = System.Text.Encoding.UTF8.GetString(htmlBytes);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                //Page Title: Meta Title, if not found then first H1, if not found then first H2, else first <title></title>
                var pageTitle = string.Empty;

                // /html/head/meta[@name="title"]/@content
                var metaTitleNodes = htmlDocument.DocumentNode.SelectNodes("//html//head//meta[@name=\"title\"]");
                if (metaTitleNodes != null)
                {
                    var metaTitleNode = metaTitleNodes.FirstOrDefault();
                    var mT = metaTitleNode?.Attributes["content"].Value;
                    if (!string.IsNullOrEmpty(mT))
                    {
                        pageTitle = mT;
                    }
                }

                if (string.IsNullOrEmpty(pageTitle))
                {
                    var h1 = htmlDocument.DocumentNode.SelectNodes("//h1");
                    if (h1 != null)
                    {
                        pageTitle = h1.FirstOrDefault().GetAllInnerTexts();
                    }
                    else
                    {
                        var h2 = htmlDocument.DocumentNode.SelectNodes("//h2");
                        if (h2 != null)
                        {
                            pageTitle = h2.FirstOrDefault().GetAllInnerTexts();
                        }
                        else
                        {
                            var title = htmlDocument.DocumentNode.SelectNodes("//title");
                            if (title != null)
                            {
                                pageTitle = title.FirstOrDefault().GetAllInnerTexts();
                            }
                        }
                    }
                }

                // /html/head/meta[@name="description"]/@content
                var metaDescription = string.Empty;
                var metaDescNodes = htmlDocument.DocumentNode.SelectNodes("//html//head//meta[@name=\"description\"]");
                if (metaDescNodes != null)
                {
                    var metaDescNode = metaDescNodes.FirstOrDefault();
                    var mD = metaDescNode?.Attributes["content"].Value;
                    if (!string.IsNullOrEmpty(mD))
                    {
                        metaDescription = mD;
                    }
                }

                // Strip out all the html tags, so we can index just the text
                var mainContainer = htmlDocument.DocumentNode.Descendants("body").FirstOrDefault();
                var content = mainContainer?.GetAllInnerTexts();

                //Check for Preferred Search Item entry
                if (!_preferredSearchItems.ContainsKey(url))
                {
                    return new SearchWebPageItem
                    {
                        Url = url,
                        PageTitle = pageTitle,
                        Content = content,
                        MetaDescription = metaDescription,
                        PreferredSearchItem = false,
                        PreferredKeywords = null,
                        OverrideTitle = null,
                        OverrideDescription = null
                    };
                }

                var pSi = _preferredSearchItems[url];
                const bool preferredSeachItem = true;
                var preferredKeywords = pSi.PreferredKeywords;
                var overrideTitle = pSi.OverrideTitle;
                var overrideDesc = pSi.OverrideDescription;

                return new SearchWebPageItem
                {
                    Url = url,
                    PageTitle = pageTitle,
                    Content = content,
                    MetaDescription = metaDescription,
                    PreferredSearchItem = preferredSeachItem,
                    PreferredKeywords = preferredKeywords,
                    OverrideTitle = overrideTitle,
                    OverrideDescription = overrideDesc
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static List<PreferredSearchItem> GetPreferredSearchItems()
        {
            var predicate = PredicateBuilder.True<PreferredSearchItem>();
            //predicate = predicate.And(e => e.TemplateName.Equals("Preferred Search Item"));
            predicate = predicate.And(e => e.TemplateName == "Preferred Search Item");
            predicate = predicate.And(e => e.PreferredKeywords != null);
            predicate = predicate.And(e => e.LatestVersion);

            var results = IndexSearcher.Search<PreferredSearchItem, DateTime>(predicate, null, SearchSortDirection.Descending, 0, 0, "web");
            //var list = results.Hits.Select(e => e.Document.GetItem()).Where(e => e != null).ToList();
            var preferredSearchItemList = results.Hits.Select(e => e.Document).Where(e => e != null).ToList();

            return preferredSearchItemList;

        }

        public void LoadPreferredSearchItems()
        {
            var pSi = GetPreferredSearchItems();

            _preferredSearchItems = new Dictionary<string, PreferredSearchItem>(pSi.Count);
            foreach (var item in pSi.Where(item => !string.IsNullOrEmpty(item.PreferredKeywords)))
            {
                try
                {
                    var tmpUrl = item.PreferredUrl;
                    var doc = new XmlDocument();
                    doc.LoadXml(tmpUrl);
                    var root = doc.DocumentElement;
                    var urlType = root?.Attributes["linktype"].Value;
                    string path;
                    if ("internal".Equals(urlType?.ToLower()))
                    {
                        var id = root.Attributes["id"].Value;
                        var tmpItem = Db.GetItem(id);
                        path = tmpItem.Paths.Path.ToLower();
                        _preferredSearchItems.Add(path, item);
                    }
                    else if ("external".Equals(urlType?.ToLower()))
                    {
                        path = root.Attributes["url"].Value;
                        if (!string.IsNullOrEmpty(path))
                        {
                            path = path.ToLower();
                        }

                        _preferredSearchItems.Add(path, item);
                    }

                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private static List<SearchExclusionItem> GetSearchExclusionItems()
        {
            var predicate = PredicateBuilder.True<SearchExclusionItem>();
            predicate = predicate.And(e => e.TemplateName == "Search Index Exclusion Item");
            predicate = predicate.And(e => e.ExclusionText != null);
            predicate = predicate.And(e => e.LatestVersion);

            var results = IndexSearcher.Search<SearchExclusionItem, DateTime>(predicate, null, SearchSortDirection.Descending, 0, 0, "web");
            var searchExclusionItemList = results.Hits.Select(e => e.Document).Where(e => e != null).ToList();

            return searchExclusionItemList;
        }

        private static IEnumerable<string> GetSearchExclusions()
        {
            var exclusionItems = GetSearchExclusionItems();
            var exclusions = exclusionItems.Where(item => !string.IsNullOrEmpty(item.ExclusionText)).Select(item => item.ExclusionText).ToList();
            return exclusions;
        }
    }
}