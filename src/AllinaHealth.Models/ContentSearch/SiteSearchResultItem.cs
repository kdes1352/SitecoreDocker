using System.Collections.Generic;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;

namespace AllinaHealth.Models.ContentSearch
{
    public class SiteSearchResultItem : SearchResultItem
    {
        [IndexField("content_t")]
        public virtual string HtmlContent { get; set; }

        [IndexField("pagetitle_t")]
        public virtual string PageTitle { get; set; }

        [IndexField("url_t")]
        public virtual string PageUrl { get; set; }

        [IndexField("metadescription_t")]
        public virtual string MetaDescription { get; set; }

        public virtual Dictionary<string, string> Highlights { get; set; }

        public SiteSearchResultItem()
        {
            Highlights = new Dictionary<string, string>();
        }
    }
}
