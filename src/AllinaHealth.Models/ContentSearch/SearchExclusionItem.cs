using Sitecore.ContentSearch;

namespace AllinaHealth.Models.ContentSearch
{
    public class SearchExclusionItem : BaseSearchResultItem
    {
        [IndexField("exclusion_text_t")]
        public string ExclusionText { get; set; }
    }
}
