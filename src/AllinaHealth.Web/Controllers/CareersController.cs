using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AllinaHealth.Framework.ContentSearch;
using AllinaHealth.Models.Allina_Health.Pages;
using AllinaHealth.Models.Careers;
using AllinaHealth.Models.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data.Items;

namespace AllinaHealth.Web.Controllers
{
    public class CareersController : Controller
    {
        public ActionResult CareersLandingPage()
        {
            return View("~/views/Careers/CareersLandingPage.cshtml");
        }

        public ActionResult CareersSubpage()
        {
            return View("~/views/Careers/CareersSubpage.cshtml");
        }

        public ActionResult CareersFeed()
        {
            var model = new CareersModel
            {
                NewsList = GetFeedNewsList()
            };

            return View("~/Views/Careers/CareersFeed.cshtml", model);
        }

        public List<Item> GetFeedNewsList()
        {
            var predicate = PredicateBuilder.True<NewsroomSearchResultItem>();
            const int take = 4;

            predicate = predicate.And(e => e.TemplateId == INews_Article_PageConstants.TemplateId).And(e => e.LatestVersion);

            Expression<Func<NewsroomSearchResultItem, DateTime>> order = e => e.ArticleDate;
            var results = IndexSearcher.Search(predicate, order, SearchSortDirection.Descending, take);
            var newsFeedList = results.Hits.Select(e => e.Document.GetItem()).Where(e => e != null).ToList();

            return newsFeedList;
        }
    }
}