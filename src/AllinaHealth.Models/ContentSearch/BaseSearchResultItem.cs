using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;

namespace AllinaHealth.Models.ContentSearch
{
    public class BaseSearchResultItem : SearchResultItem
    {
        [IndexField("_latestversion")]
        public virtual bool LatestVersion { get; set; }
    }
}