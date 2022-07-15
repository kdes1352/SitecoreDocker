using Sitecore.ContentSearch;

namespace AllinaHealth.Models.ContentSearch
{
    public class PreferredSearchItem : BaseSearchResultItem
    {
        [IndexField("preferred_url_t")]
        public string PreferredUrl { get; set; }

        [IndexField("preferred_keywords_t")]
        public string PreferredKeywords { get; set; }

        [IndexField("override_title_t")]
        public string OverrideTitle { get; set; }

        [IndexField("override_description_t")]
        public string OverrideDescription { get; set; }

    }
}
