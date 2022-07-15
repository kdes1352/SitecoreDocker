using System;
using System.Collections.Generic;
using Sitecore.ContentSearch;
using Sitecore.Data;

namespace AllinaHealth.Models.ContentSearch
{
    public class NewsroomSearchResultItem : BaseSearchResultItem
    {
        [IndexField("article_date_tdt")]
        public virtual DateTime ArticleDate { get; set; }

        [IndexField("categories_sm")]
        public virtual IEnumerable<ID> Categories { get; set; }

        [IndexField("regions_sm")]
        public virtual IEnumerable<ID> Regions { get; set; }
    }
}