using Sitecore.ContentSearch;

namespace AllinaHealth.Models.ContentSearch
{
    public class SitemapSearchResultItem : BaseSearchResultItem
    {
        [IndexField("include_in_sitemap_b")]
        public virtual bool IncludeInSitemap { get; set; }
    }
}
