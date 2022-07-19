using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AllinaHealth.Models.ContentSearch;
using AllinaHealth.Models.ViewModels.Search;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SolrProvider.SolrNetIntegration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Common;
using Sitecore.Mvc.Pipelines.Response.RenderPlaceholder;
using Sitecore.Mvc.Presentation;
using Sitecore.Pipelines;
using SolrNet.Commands.Parameters;

namespace AllinaHealth.Web.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult Search(string q, int pg = 1)
        {
            if (string.IsNullOrEmpty(q))
            {
                var sp = new SearchPage();
                return View("~/Views/Search/Results.cshtml", sp);
            }

            const int pageSize = 10;

            ////Decode the query string for actual search, keep encoded format for future search requests
            var query = Server.HtmlDecode(q);

            var skip = pageSize * (pg - 1);

            var index = ContentSearchManager.GetIndex("search_index");
            var highlightFields = new Collection<string>
            {
                "pagetitle_t",
                "content_t",
                "metadescription_t"
            };

            SearchPage model = null;
            using (var context = index.CreateSearchContext())
            {
                try
                {
                    //Surround query with parentheses or double quotes to treat as a phrase
                    var solrQuery = "(" + query + ")";

                    var results = context.Query<SiteSearchResultItem>($"metadescription_t:{solrQuery} OR pagetitle_t:{solrQuery} OR content_t:{solrQuery}", new QueryOptions
                    {
                        Highlight = new HighlightingParameters
                        {
                            Fields = highlightFields,
                            BeforeTerm = "<strong>",
                            AfterTerm = "</strong>"
                        },
                        ExtraParams = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("hl.method", "unified"), new KeyValuePair<string, string>("hl.bs.type", "WHOLE")
                        },
                        Start = skip,
                        Rows = pageSize
                    });
                    if (results != null)
                    {
                        foreach (var result in results)
                        {
                            foreach (var hl in highlightFields)
                            {
                                try
                                {
                                    result.Highlights[hl] = string.Join(", ", results.Highlights[result["_uniqueid"]].Snippets[hl].ToArray());
                                }
                                catch (Exception)
                                {
                                    //To-Do: Log Error???
                                }
                            }
                        }
                    }

                    var preferredResults = new List<SiteSearchPreferredResultItem>();
                    if (skip == 0)
                    {
                        //Preferred Keyword is indexed by phrase and not by word, need to double quote search term to receive a match
                        var solrPrefQuery = "(\"" + query + "\")";
                        var tmpPrefResults = context.Query<SiteSearchPreferredResultItem>($"preferredsearchitem_b:true AND preferredkeywords_t:{solrPrefQuery}", new QueryOptions
                        {
                            Start = 0,
                            Rows = pageSize
                        });
                        if (null != tmpPrefResults && tmpPrefResults.Any())
                        {
                            preferredResults = tmpPrefResults.ToList();
                        }
                    }

                    if (results != null)
                    {
                        model = new SearchPage(Server.HtmlDecode(query), results.ToList(), preferredResults, results.NumFound, pageSize, pg);
                    }
                }
                catch (Exception)
                {
                    //TO-DO: Log Error
                }

                return View("~/Views/Search/Results.cshtml", model ?? new SearchPage());
            }
        }

        public string GetHtmlForIndex(string language, string database, string itemId)
        {
            try
            {
                var s = string.Empty;
                var l = LanguageManager.GetLanguage(language);
                var id = ID.Parse(itemId);
                var db = Factory.GetDatabase(database);
                var i = db.GetItem(id, l);
                using (new ContextItemSwitcher(i))
                {
                    if (!(PageContext.Current.PageView is RenderingView))
                    {
                        return s;
                    }

                    var r = new Rendering();
                    using (var sw = new StringWriter())
                    {
                        using (var args = new RenderPlaceholderArgs("Layout", sw, r))
                        {
                            var viewContext = new ViewContext(ControllerContext, new RenderingView(r), new ViewDataDictionary(), new TempDataDictionary(), sw);
                            using (ContextService.Get().Push(viewContext))
                            {
                                CorePipeline.Run("mvc.renderPlaceholder", args);
                            }
                        }

                        s = sw.ToString();
                    }
                }

                return s;
            }
            catch (Exception exc)
            {
                Log.Error($"SearchIndexController - Exception - {itemId}: {exc.Message}", exc, this);
                return " ";
            }
        }
    }
}