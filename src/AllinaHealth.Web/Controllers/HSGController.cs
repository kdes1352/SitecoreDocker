using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AllinaHealth.Framework.ContentSearch;
using AllinaHealth.Models.Allina_Health.Pages.HSG;
using AllinaHealth.Models.ContentSearch;
using AllinaHealth.Models.Extensions;
using AllinaHealth.Models.ViewModels.HSG;
using Sitecore;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data;
using Sitecore.Mvc.Presentation;
using Sitecore.Text;

namespace AllinaHealth.Web.Controllers
{
    // ReSharper disable once InconsistentNaming
    public class HSGController : Controller
    {
        public ActionResult BlogList()
        {
            var predicate = PredicateBuilder.True<HSGSearchResultItem>();
            var take = RenderingContext.Current.Rendering.Item.GetFieldInteger("Max Number of Articles", int.MaxValue);
            var categories = RenderingContext.Current.Rendering.Item.GetFieldValue("Categories");
            predicate = predicate.And(e => e.TemplateId == IHSG_Articles_PageConstants.TemplateId);
            predicate = predicate.And(e => e.LatestVersion);

            if (!string.IsNullOrEmpty(categories))
            {
                var idList = new ListString(categories);
                var predicateOr = PredicateBuilder.False<HSGSearchResultItem>();
                predicateOr = idList.Select(c => new ID(c).ToString()).Aggregate(predicateOr, (current, temp) => current.Or(e => e.Category == temp));
                predicate = predicate.And(predicateOr);
            }

            Expression<Func<HSGSearchResultItem, DateTime>> order = e => e.PostedDate;
            var results = IndexSearcher.Search(predicate, order, SearchSortDirection.Descending, take);
            var list = results.Hits.Select(e => e.Document.GetItem()).Where(e => e != null).ToList();

            return View("~/Views/HSG/BlogList.cshtml", list);
        }


        // ReSharper disable once InconsistentNaming
        public ActionResult HSGCategory()
        {
            return View("~/views/HSG/HSGCategories.cshtml", new CategoryViewModel());
        }

        // ReSharper disable once InconsistentNaming
        public ActionResult HSGArticle()
        {
            return View("~/views/HSG/HSGArticles.cshtml", new ArticleViewModel(Context.Item));
        }

        // ReSharper disable once InconsistentNaming
        public ActionResult HSGNavigation()
        {
            return View("~/views/HSG/Partials/Navigation.cshtml", new NavigationViewModel());
        }

        // ReSharper disable once InconsistentNaming
        public ActionResult ExploreHSG()
        {
            return View("~/views/HSG/Partials/ExploreHSG.cshtml", new NavigationViewModel());
        }

        // ReSharper disable once InconsistentNaming
        public ActionResult HSGAuthor()
        {
            return View("~/views/HSG/Partials/Author.cshtml");
        }

        // ReSharper disable once InconsistentNaming
        public ActionResult HSGRelatedCallsToAction()
        {
            return View("~/views/HSG/Partials/RelatedCallsToAction.cshtml");
        }

        // ReSharper disable once InconsistentNaming
        public ActionResult HSGQuickFact()
        {
            return View("~/views/HSG/Partials/QuickFacts.cshtml");
        }
    }
}