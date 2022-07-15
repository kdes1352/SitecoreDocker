using System;
using System.Collections.Generic;
using System.Linq;
using AllinaHealth.Models.ContentSearch;
using AllinaHealth.Models.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data.Items;

namespace AllinaHealth.Models.ViewModels.HSG
{
    public class CategoryViewModel
    {
        private readonly Item _categoryItem;
        private string _className;
        private string _topicClassName;
        private List<ArticleViewModel> _list;

        private List<ArticleViewModel> GetRelatedArticles()
        {
            List<ArticleViewModel> list;
            var predicate = PredicateBuilder.True<HSGSearchResultItem>();
            predicate = predicate.And(e => e.Path.StartsWith(CategoryItem.Paths.FullPath, StringComparison.InvariantCultureIgnoreCase));
            predicate = predicate.And(e => e.TemplateId == Allina_Health.Pages.HSG.IHSG_Articles_PageConstants.TemplateId);
            predicate = predicate.And(e => e.LatestVersion);
            var indexName = $"sitecore_{Sitecore.Context.Database.Name}_index";
            using (var searchContext = ContentSearchManager.GetIndex(indexName).CreateSearchContext())
            {
                IQueryable<HSGSearchResultItem> queryableResult = searchContext.GetQueryable<HSGSearchResultItem>().Where(predicate).OrderByDescending(e => e.PostedDate);

                list = queryableResult.ToList().Select(e => new ArticleViewModel(e.GetItem())).ToList();
            }

            return list;
        }

        public List<ArticleViewModel> RelatedArticles => _list ?? (_list = GetRelatedArticles());

        public Item CategoryItem => _categoryItem;

        public CategoryViewModel()
        {
            _categoryItem = Sitecore.Context.Item;
        }

        public CategoryViewModel(Item i)
        {
            _categoryItem = i;
        }

        public string ClassName
        {
            get
            {
                if (string.IsNullOrEmpty(_className))
                {
                    _className = _categoryItem.GetInternalLinkFieldItem("Category").GetFieldValue("Class Name");
                }

                return _className;
            }
        }

        public string TopicClassName
        {
            get
            {
                if (string.IsNullOrEmpty(_topicClassName))
                {
                    _topicClassName = _categoryItem.GetInternalLinkFieldItem("Category").GetFieldValue("Topic Class Name");
                }

                return _topicClassName;
            }
        }
    }
}