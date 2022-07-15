using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch;

namespace AllinaHealth.Framework.ContentSearch.GlobalSearch
{
    public class SearchWebPageCrawler : FlatDataCrawler<IndexableSearchWebPage>
    {
        private readonly SearchWebPageRepository _repository;

        public SearchWebPageCrawler()
        {
            _repository = new SearchWebPageRepository();
        }

        public SearchWebPageCrawler(IIndexOperations indexOperations) : base(indexOperations)
        {
            _repository = new SearchWebPageRepository();
        }

        protected override IndexableSearchWebPage GetIndexable(IIndexableUniqueId indexableUniqueId)
        {
            return null;
        }

        protected override IndexableSearchWebPage GetIndexableAndCheckDeletes(IIndexableUniqueId indexableUniqueId)
        {
            return null;
        }

        protected override IEnumerable<IIndexableUniqueId> GetIndexablesToUpdateOnDelete(IIndexableUniqueId indexableUniqueId)
        {
            return null;
        }

        protected override IEnumerable<IndexableSearchWebPage> GetItemsToIndex()
        {
            return _repository.GetAllSearchPages().Select(e => new IndexableSearchWebPage(e));
        }

        protected override bool IndexUpdateNeedDelete(IndexableSearchWebPage indexable)
        {
            return _repository.UrlList.All(e => e != indexable.Id.Value.ToString());
        }
    }
}