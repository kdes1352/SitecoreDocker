using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AllinaHealth.Framework.ContentSearch;
using AllinaHealth.Models.Allina_Health.Pages.News_Blog;
using AllinaHealth.Models.Allina_Health.Reference_Data.News_Blog;
using AllinaHealth.Models.ContentSearch;
using AllinaHealth.Models.Contexts;
using AllinaHealth.Models.Extensions;
using AllinaHealth.Models.ViewModels.NewsBlog;
using HtmlAgilityPack;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SolrProvider.SolrNetIntegration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using SolrNet.Commands.Parameters;

namespace AllinaHealth.Web.Controllers
{
    public class NewsBlogController : Controller
    {
        public ActionResult NewsPost()
        {
            var menu = BuildNewsBlogMenu();
            var model = new PostViewModel(Context.Item, menu);

            //Exract the photo width
            if (null != Context.Item.Fields["Photo"] && Context.Item.Fields["Photo"].HasValue)
            {
                model.PhotoWidth = ((ImageField)Context.Item.Fields["Photo"]).Width;
            }
            else if (null != Context.Item.Fields["External Photo"] && Context.Item.Fields["External Photo"].HasValue)
            {
                var imgHtml = Context.Item.Fields["External Photo"].Value.ToLower();
                if (imgHtml.IndexOf("width", StringComparison.Ordinal) <= -1) return View("~/Views/NewsBlog/NewsBlogPosts.cshtml", model);
                var imgHtmlDoc = new HtmlDocument();
                imgHtmlDoc.LoadHtml(imgHtml);
                var imgNodes = imgHtmlDoc.DocumentNode.SelectNodes("//img");
                if (null == imgNodes)
                {
                    return View("~/Views/NewsBlog/NewsBlogPosts.cshtml", model);
                }

                foreach (var img in imgNodes)
                {
                    var widthAtt = img.Attributes["width"];
                    if (null != widthAtt)
                    {
                        model.PhotoWidth = widthAtt.Value;
                    }
                }

            }

            return View("~/Views/NewsBlog/NewsBlogPosts.cshtml", model);
        }

        public ActionResult NewsPostHome(int pg = 1)
        {
            var model = new HomePageViewModel
            {
                MenuTree = BuildNewsBlogMenu()
            };
            var predicate = PredicateBuilder.True<NewsBlogPostResultItem>();
            //Number of returns plus one for top post and then next specified grouping of results
            var numResults = RenderingContext.Current.Rendering.Item.GetFieldInteger("Number Of Posts", 10);
            //var take = numResults + 1;
            predicate = predicate.And(e => e.TemplateId == INews_Blog_Post_PageConstants.TemplateId);
            predicate = predicate.And(e => e.LatestVersion);

            model.Page = pg;
            var skip = (pg > 1 ? (numResults * (pg - 1)) : 0) + 1;

            Expression<Func<NewsBlogPostResultItem, DateTime>> order = e => e.PublishDate;

            //Get the latest post
            var latestPostResults = IndexSearcher.Search(predicate, order, SearchSortDirection.Descending, 1);
            var latestPost = latestPostResults.Hits.Select(e => e.Document.GetItem()).FirstOrDefault(e => e != null);
            if (null != latestPost)
            {
                model.NewsBlogPostItem = latestPost;
            }

            //Get the results
            var results = IndexSearcher.Search(predicate, order, SearchSortDirection.Descending, numResults, skip);
            var total = results.TotalSearchResults;
            var list = results.Hits.Select(e => e.Document.GetItem()).Where(e => e != null).ToList();

            //if (null != list && list.Count > 0)
            //{
            //    model.NewsBlogPostItem = list.ElementAt(0);
            //    if (list.Count > 1 && list.Count < take)
            //    {
            //        model.SetTopPosts(list.GetRange(1, list.Count - 1));
            //    }
            //    else
            //    {
            //        model.SetTopPosts(list.GetRange(1, numResults));
            //    }
            //}

            model.SetTopPosts(list);
            model.HasMoreResults = ((pg * numResults) + 1) < total;

            //Experimental stuff, may stick around, may get wiped out.... Time will tell
            //Database master = Sitecore.Configuration.Factory.GetDatabase("web");
            //Item home = master.GetItem(Sitecore.Context.Item.ID);
            //var urlOptions = UrlOptions.DefaultOptions;
            //urlOptions.AlwaysIncludeServerUrl = true;

            ////RenderingContext.Current.Rendering.Item.Fields["Canonical Current URL"] = "Hello World";

            //var cURL_Base = HttpUtility.UrlEncode(LinkManager.GetItemUrl(Sitecore.Context.Item, urlOptions)).ToLower().Replace("%2f", "/").Replace("%3a", ":");
            //if (home.Fields["Canonical Current URL"].Value == "" && home.Fields["Canonical Next URL"].Value == "" && home.Fields["Canonical Previous URL"].Value == "")
            //{

            //    //home["Canonical Current URL"] = cURL_Base + "?pg=" + model.Page + "#results";

            //    using (new Sitecore.SecurityModel.SecurityDisabler())
            //    {
            //        var ccURL = cURL_Base + "?pg=" + model.Page + "#results";
            //        var cnURL = model.HasMoreResults ? cURL_Base + "?pg=" + (model.Page + 1) + "#results" : ccURL;
            //        var cpURL = model.Page > 1 ? cURL_Base + "?pg=" + (model.Page - 1) + "#results" : ccURL;

            //        //using (new EditContext(home))
            //        //{
            //        //    home.Fields["Canonical Current URL"].SetValue(ccURL, true);
            //        //    home.Fields["Canonical Next URL"].SetValue(cnURL, true);
            //        //    home.Fields["Canonical Previous URL"].SetValue(cpURL, true);
            //        //}
            //        home.Editing.BeginEdit();
            //        home["Canonical Current URL"] = ccURL;
            //        home["Canonical Next URL"] = cnURL;
            //        home["Canonical Previous URL"] = cpURL;
            //        home.Editing.EndEdit();
            //    }
            //}
            //else
            //{
            //    using (new Sitecore.SecurityModel.SecurityDisabler())
            //    {
            //        home.Editing.BeginEdit();
            //        home["Canonical Current URL"] = "";
            //        home["Canonical Next URL"] = "";
            //        home["Canonical Previous URL"] = "";
            //        home.Editing.EndEdit();
            //    }
            //}

            return View("~/Views/NewsBlog/NewsBlogHome.cshtml", model);
        }

        //Nuke this....
        //public ActionResult Reset_SEO_URLs()
        //{
        //    Database master = Sitecore.Configuration.Factory.GetDatabase("master");
        //    Item home = master.GetItem(Sitecore.Context.Item.ID);
        //    var urlOptions = UrlOptions.DefaultOptions;
        //    urlOptions.AlwaysIncludeServerUrl = true;

        //    using (new Sitecore.SecurityModel.SecurityDisabler())
        //    {
        //        home.Editing.BeginEdit();
        //        home["Canonical Current URL"] = "";
        //        home["Canonical Next URL"] = "";
        //        home["Canonical Previous URL"] = "";
        //        home.Editing.EndEdit();
        //    }

        //    return null;
        //}

        public ActionResult NewsPostMonth(int pg = 1)
        {
            var item = Context.Item;

            var model = new MonthPageViewModel(item)
            {
                MenuTree = BuildNewsBlogMenu()
            };

            var total = item.GetChildrenSafe().Count;
            var numResults = item.GetFieldInteger("Number Of Posts", 10);

            model.Page = pg;
            if (pg > 1 && ((pg * numResults) > (total + numResults)))
            {
                return View("~/Views/NewsBlog/NewsBlogMonth.cshtml", model);
            }

            var skip = (pg > 1 ? (numResults * (pg - 1)) : 0);
            var take = (((skip + numResults) < total) ? numResults : (total - skip));

            var predicate = PredicateBuilder.True<NewsBlogPostResultItem>();
            predicate = predicate.And(e => e.TemplateId == INews_Blog_Post_PageConstants.TemplateId);
            predicate = predicate.And(e => e.Paths.Contains(item.ID));
            predicate = predicate.And(e => e.LatestVersion);

            Expression<Func<NewsBlogPostResultItem, DateTime>> order = e => e.PublishDate;
            var results = IndexSearcher.Search(predicate, order, SearchSortDirection.Descending, take, skip);
            var list = results.Hits.Select(e => e.Document.GetItem()).Where(e => e != null).ToList();

            model.SetPosts(list);
            model.HasMoreResults = (pg * numResults) < total;

            return View("~/Views/NewsBlog/NewsBlogMonth.cshtml", model);
        }

        public ActionResult NewsPostYear(int pg = 1)
        {
            var item = Context.Item;
            var childTemplateId = INews_Blog_Post_PageConstants.TemplateId;

            var model = new YearPageViewModel(item)
            {
                MenuTree = BuildNewsBlogMenu()
            };

            //Get the total number of possible posts
            var total = item.Axes.GetDescendants().Count(x => x.TemplateID == childTemplateId);
            var numResults = item.GetFieldInteger("Number Of Posts", 10);

            model.Page = pg;
            if (pg > 1 && ((pg * numResults) > (total + numResults)))
            {
                return View("~/Views/NewsBlog/NewsBlogYear.cshtml", model);
            }

            var skip = (pg > 1 ? (numResults * (pg - 1)) : 0);
            var take = (((skip + numResults) < total) ? numResults : (total - skip));

            var predicate = PredicateBuilder.True<NewsBlogPostResultItem>();
            predicate = predicate.And(e => e.TemplateId == INews_Blog_Post_PageConstants.TemplateId);
            predicate = predicate.And(e => e.Paths.Contains(item.ID));
            predicate = predicate.And(e => e.LatestVersion);

            Expression<Func<NewsBlogPostResultItem, DateTime>> order = e => e.PublishDate;
            var results = IndexSearcher.Search(predicate, order, SearchSortDirection.Descending, take, skip);
            var list = results.Hits.Select(e => e.Document.GetItem()).Where(e => e != null).ToList();

            model.SetPosts(list);
            model.HasMoreResults = (pg * numResults) < total;

            return View("~/Views/NewsBlog/NewsBlogYear.cshtml", model);
        }

        public ActionResult NewsPostCategory(string catid = "", int pg = 1)
        {
            var item = Context.Item;
            ID catIdObj;
            var model = new SearchByRelatedItemPageViewModel(item)
            {
                SearchItemID = catid,
                MenuTree = BuildNewsBlogMenu(catid)
            };

            if (string.IsNullOrEmpty(catid))
            {
                model.Page = 1;
                return View("~/Views/NewsBlog/NewsBlogCategory.cshtml", model);
            }

            try
            {
                catIdObj = ID.Parse(catid);
            }
            catch
            {
                model.Page = 1;
                return View("~/Views/NewsBlog/NewsBlogCategory.cshtml", model);
            }

            //Get the total number of possible posts
            var numResults = item.GetFieldInteger("Number Of Posts", 10);
            var predicate = PredicateBuilder.True<NewsBlogPostResultItem>();
            predicate = predicate.And(e => e.TemplateId == INews_Blog_Post_PageConstants.TemplateId);
            predicate = predicate.And(e => e.LatestVersion);
            predicate = predicate.And(p => p.Categories.Contains(catIdObj));

            model.Page = pg;
            var skip = (pg > 1 ? (numResults * (pg - 1)) : 0);

            Expression<Func<NewsBlogPostResultItem, DateTime>> order = e => e.PublishDate;
            var results = IndexSearcher.Search(predicate, order, SearchSortDirection.Descending, numResults, skip);
            var total = results.TotalSearchResults;
            var list = results.Hits.Select(e => e.Document.GetItem()).Where(e => e != null).ToList();

            model.SetPosts(list);
            model.HasMoreResults = (pg * numResults) < total;

            return View("~/Views/NewsBlog/NewsBlogCategory.cshtml", model);
        }

        public ActionResult NewsPostKeyword(string kwid = "", int pg = 1)
        {
            var item = Context.Item;
            ID kwIdObj;
            var model = new SearchByRelatedItemPageViewModel(item)
            {
                SearchItemID = kwid,
                MenuTree = BuildNewsBlogMenu()
            };

            if (string.IsNullOrEmpty(kwid))
            {
                model.Page = 1;
                return View("~/Views/NewsBlog/NewsBlogKeyword.cshtml", model);
            }

            try
            {
                kwIdObj = ID.Parse(kwid);
            }
            catch
            {
                model.Page = 1;
                return View("~/Views/NewsBlog/NewsBlogKeyword.cshtml", model);
            }

            //Get the total number of possible posts
            var numResults = item.GetFieldInteger("Number Of Posts", 10);
            var predicate = PredicateBuilder.True<NewsBlogPostResultItem>();
            predicate = predicate.And(e => e.TemplateId == INews_Blog_Post_PageConstants.TemplateId);
            predicate = predicate.And(e => e.LatestVersion);
            predicate = predicate.And(p => p.Keywords.Contains(kwIdObj));

            model.Page = pg;
            var skip = (pg > 1 ? (numResults * (pg - 1)) : 0);

            Expression<Func<NewsBlogPostResultItem, DateTime>> order = e => e.PublishDate;
            var results = IndexSearcher.Search(predicate, order, SearchSortDirection.Descending, numResults, skip);
            var total = results.TotalSearchResults;
            var list = results.Hits.Select(e => e.Document.GetItem()).Where(e => e != null).ToList();

            model.SetPosts(list);
            model.HasMoreResults = (pg * numResults) < total;

            return View("~/Views/NewsBlog/NewsBlogKeyword.cshtml", model);
        }

        public ActionResult NewsPostSearch(string q, int pg = 1)
        {
            var model = new SearchPageViewModel
            {
                MenuTree = BuildNewsBlogMenu()
            };

            if (string.IsNullOrEmpty(q))
            {
                return View("~/Views/NewsBlog/NewsBlogSearchResults.cshtml", model);
            }

            var item = Context.Item;
            var numResults = item.GetFieldInteger("Number Of Posts", 10);

            var templateId = INews_Blog_Post_PageConstants.TemplateId.ToShortID().ToString();
            model.Page = pg;
            var skip = (pg > 1 ? (numResults * (pg - 1)) : 0);

            ////Decode the query string for actual search, keep encoded format for future search requests
            var query = Server.HtmlDecode(q);

            var index = ContentSearchManager.GetIndex("sitecore_web_index");
            var highlightFields = new Collection<string>
            {
                "title_t",
                "content_t",
                "meta_description_t"
            };

            using (var context = index.CreateSearchContext())
            {
                try
                {
                    //Surround query with parentheses or double quotes to treat as a phrase
                    var solrQuery = "(" + query + ")";
                    var results = context.Query<NewsBlogPostResultItem>($"(content_t:{solrQuery} OR title_t:{solrQuery} OR meta_description_t:{solrQuery}) AND _latestversion:true AND _template:{templateId}", new QueryOptions
                    {
                        Highlight = new HighlightingParameters
                        {
                            Fields = highlightFields,
                            BeforeTerm = "<strong>",
                            AfterTerm = "</strong>"
                        },
                        ExtraParams = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("hl.method", "unified")
                        },
                        Start = skip,
                        Rows = numResults
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

                    if (results != null)
                    {
                        model.Posts = results.ToList();
                        model.SearchText = Server.HtmlDecode(query);
                        model.TotalHits = results.NumFound;
                        model.UpdatePager(results.NumFound, numResults, pg);
                    }
                }
                catch (Exception)
                {
                    //TO-DO: Log Error
                }
            }

            return View("~/Views/NewsBlog/NewsBlogSearchResults.cshtml", (model));
        }

        private static NewsBlogMenuTreeModel BuildNewsBlogMenu(string categoryId = null)
        {
            var item = Context.Item;
            var allinaNewsTemplateId = INews_Blog_Home_PageConstants.TemplateId;
            var newsBlogHome = item.GetFirstParentOfTemplate(allinaNewsTemplateId.ToString());

            if (null == newsBlogHome)
            {
                return new NewsBlogMenuTreeModel();
            }

            var context = new SiteContextModel();
            var yearPageTemplateId = INews_Blog_Year_PageConstants.TemplateId;
            var root = new NewsBlogMenuTreeModel(newsBlogHome, false)
            {
                Title = context.GetDictionaryText("NewsBlog:MenuTitle"), //Maybe set this in the page data
                URL = LinkManager.GetItemUrl(newsBlogHome)
            };

            var predicate = PredicateBuilder.True<NewsBlogCategoryResultItem>();
            predicate = predicate.And(e => e.TemplateId == INews_Blog_CategoryConstants.TemplateId);
            predicate = predicate.And(e => e.ShowInMenu == true);
            Expression<Func<NewsBlogCategoryResultItem, string>> order = e => e.CategorySort;
            var results = IndexSearcher.Search(predicate, order);
            var categories = results.Hits.Select(e => e.Document.GetItem()).Where(e => e != null).ToList();

            var subjectNode = new NewsBlogMenuTreeModel(context.GetDictionaryText("NewsBlog:MenuSubjectBranchTitle"), null, root);
            var yearNode = new NewsBlogMenuTreeModel(context.GetDictionaryText("NewsBlog:MenuYearBranchTitle"), null, root);

            var categoryLink = context.GetDictionaryText("NewsBlog:MenuCategoryURL");
            foreach (var category in categories)
            {
                if (null == category)
                {
                    continue;
                }

                var node = new NewsBlogMenuTreeModel
                {
                    Title = (category.GetFieldValue("Category")),
                    URL = $"{categoryLink}?catid={category.ID.ToShortID().ToString().ToLower()}"
                };
                if (category.ID.ToShortID().ToString().ToLower().Equals(categoryId))
                {
                    //Category section of the menu is not Item based and only contains parameterized links
                    //Need to manually set the IsSelected & IsActive flags for both the node and the parent
                    node.IsSelected = true;
                    node.IsActive = true;
                    subjectNode.IsActive = true;
                }

                subjectNode.AddChild(node);
            }

            root.AddChild(subjectNode);

            var yearPages = newsBlogHome.GetChildren().Where(c => c.TemplateID.Equals(yearPageTemplateId)).OrderByDescending(x => x.Name).Take(3);
            foreach (var yearPage in yearPages)
            {
                var yearPageNode = new NewsBlogMenuTreeModel(yearPage, yearNode, false);
                var monthPages = yearPage.GetChildren().ToList();
                foreach (var monthPageNode in from monthPage in monthPages let monthNum = monthPage.Name select new NewsBlogMenuTreeModel(monthPage, GetMonth(monthNum), yearPageNode, false))
                {
                    yearPageNode.AddChild(monthPageNode);
                }

                yearNode.AddChild(yearPageNode);
            }

            root.AddChild(yearNode);
            return root;
        }

        private static string GetMonth(string monthNumber)
        {
            if (string.IsNullOrEmpty(monthNumber))
            {
                return "";
            }

            const int numItems = 12;
            var monthArray = new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            var isIndex = int.TryParse(monthNumber, out var i);
            if (isIndex && i > 0 && i <= numItems)
            {
                return monthArray[(i - 1)];
            }

            return "";
        }
    }
}