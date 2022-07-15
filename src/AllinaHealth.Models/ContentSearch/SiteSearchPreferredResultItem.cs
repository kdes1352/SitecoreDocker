using System;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;

namespace AllinaHealth.Models.ContentSearch
{
    public class SiteSearchPreferredResultItem : SearchResultItem
    {
        [IndexField("content_t")]
        public virtual string HtmlContent { get; set; }

        [IndexField("pagetitle_t")]
        public virtual string PageTitle { get; set; }

        [IndexField("url_t")]
        public virtual string PageUrl { get; set; }

        [IndexField("metadescription_t")]
        public virtual string MetaDescription { get; set; }

        [IndexField("preferredkeywords_t")]
        public virtual string PreferredKeywords { get; set; }

        [IndexField("preferredsearchitem_b")]
        public virtual Boolean PreferredSearchItem { get; set; }

        [IndexField("overridetitle_t")]
        public string OverrideTitle { get; set; }

        [IndexField("overridedescription_t")]
        public string OverrideDescription { get; set; }

        //public SiteSearchPreferredResultItem()
        //{
        //}
    }
}
