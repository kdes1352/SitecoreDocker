using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AllinaHealth.Framework.ContentSearch;
using AllinaHealth.Models.Allina_Health.Pages.Provider_Connect;
using AllinaHealth.Models.ContentSearch;
using AllinaHealth.Models.Extensions;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Mvc.Presentation;

namespace AllinaHealth.Web.Controllers
{
    public class ProviderConnectController : Controller
    {

        public ActionResult PostsList()
        {
            var predicate = PredicateBuilder.True<NewsroomSearchResultItem>();
            var take = RenderingContext.Current.Rendering.Item.GetFieldInteger("Max Number of Posts", int.MaxValue);
            var year = RenderingContext.Current.Rendering.Item.GetFieldValue("Year");

            if (int.TryParse(year, out var yearInt))
            {
                var startDate = new DateTime(yearInt, 1, 1);
                var endDate = new DateTime(yearInt + 1, 1, 1);
                predicate = predicate.And(e => e.ArticleDate >= startDate && e.ArticleDate < endDate);
            }

            predicate = predicate.And(e => e.TemplateId == IProvider_Connect_Post_PageConstants.TemplateId);
            predicate = predicate.And(e => e.LatestVersion);

            Expression<Func<NewsroomSearchResultItem, DateTime>> order = e => e.ArticleDate;
            var results = IndexSearcher.Search(predicate, order, SearchSortDirection.Descending, take);
            var list = results.Hits.Select(e => e.Document.GetItem()).Where(e => e != null).ToList();

            return View("~/Views/ProviderConnect/ProviderPostsList.cshtml", list);
        }
    }
}