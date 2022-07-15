using System;
using Sitecore.ContentSearch;

namespace AllinaHealth.Models.ContentSearch
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    public class HSGSearchResultItem : BaseSearchResultItem
    {
        [IndexField("category_t")]
        public virtual string Category { get; set; }


        [IndexField("article_posted_date_tdt")]
        public virtual DateTime PostedDate { get; set; }
    }
}
