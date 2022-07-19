using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AllinaHealth.Framework.ContentSearch;
using AllinaHealth.Models.Allina_Health.Pages;
using AllinaHealth.Models.ContentSearch;
using AllinaHealth.Models.Extensions;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data;
using Sitecore.Mvc.Presentation;
using Sitecore.Text;

namespace AllinaHealth.Web.Controllers
{
    public class NewsroomController : Controller
    {
        public ActionResult NewsArticle()
        {
            return View("~/Views/Newsroom/NewsArticle.cshtml");
        }

        public ActionResult NewsList()
        {
            var predicate = PredicateBuilder.True<NewsroomSearchResultItem>();
            var take = RenderingContext.Current.Rendering.Item.GetFieldInteger("Max Number of Articles", int.MaxValue);
            var year = RenderingContext.Current.Rendering.Item.GetFieldValue("Year");
            var categories = RenderingContext.Current.Rendering.Item.GetFieldValue("Categories");

            if (int.TryParse(year, out var yearInt))
            {
                var startDate = new DateTime(yearInt, 1, 1);
                var endDate = new DateTime(yearInt + 1, 1, 1);
                predicate = predicate.And(e => e.ArticleDate >= startDate && e.ArticleDate < endDate);
            }

            predicate = predicate.And(e => e.TemplateId == INews_Article_PageConstants.TemplateId);
            predicate = predicate.And(e => e.LatestVersion);

            if (!string.IsNullOrEmpty(categories))
            {
                var idList = new ListString(categories);
                var predicateOr = PredicateBuilder.False<NewsroomSearchResultItem>();
                predicateOr = idList.Select(c => new ID(c)).Aggregate(predicateOr, (current, temp) => current.Or(e => e.Categories.Contains(temp)));
                predicate = predicate.And(predicateOr);
            }

            Expression<Func<NewsroomSearchResultItem, DateTime>> order = e => e.ArticleDate;
            var results = IndexSearcher.Search(predicate, order, SearchSortDirection.Descending, take);
            var list = results.Hits.Select(e => e.Document.GetItem()).Where(e => e != null).ToList();

            return View("~/Views/Newsroom/NewsList.cshtml", list);
        }
    }
}