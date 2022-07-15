using System;
using System.Linq;
using System.Linq.Expressions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.SearchTypes;

namespace AllinaHealth.Framework.ContentSearch
{
    public static class IndexSearcher
    {

        public static SearchResults<T> Search<T, TSort>(Expression<Func<T, bool>> predicate, Expression<Func<T, TSort>> order = null, SearchSortDirection direction = SearchSortDirection.Ascending, int take = 0, int skip = 0, string database = null) where T : SearchResultItem
        {
            if (null == database)
            {
                database = Sitecore.Context.Database.Name;
            }

            var indexName = string.Format("sitecore_{0}_index", database);
            using (var searchContext = ContentSearchManager.GetIndex(indexName).CreateSearchContext())
            {
                var queryableResult = searchContext.GetQueryable<T>().Where(predicate);
                if (order != null)
                {
                    if (direction == SearchSortDirection.Ascending)
                    {
                        queryableResult = queryableResult.OrderBy(order);
                    }
                    else if (direction == SearchSortDirection.Descending)
                    {
                        queryableResult = queryableResult.OrderByDescending(order);
                    }
                }

                if (skip > 0)
                {
                    queryableResult = queryableResult.Skip(skip);
                }

                if (take > 0)
                {
                    queryableResult = queryableResult.Take(take);
                }

                return queryableResult.GetResults();
            }
        }
    }

    public enum SearchSortDirection
    {
        Ascending,
        Descending
    }
}