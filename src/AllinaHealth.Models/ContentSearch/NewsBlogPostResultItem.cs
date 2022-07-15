using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Converters;
using Sitecore.Data;

namespace AllinaHealth.Models.ContentSearch
{
    public class NewsBlogPostResultItem : BaseSearchResultItem
    {
        [IndexField("publish_date_tdt")]
        public virtual DateTime PublishDate { get; set; }

        [IndexField("title_t")]
        public virtual string Title { get; set; }

        [IndexField("content_t")]
        public virtual string PostContent { get; set; }

        [IndexField("meta_description_t")]
        public virtual string MetaDescription { get; set; }

        [IndexField("url_t")]
        public virtual string PageUrl { get; set; }

        [IndexField("categories_sm")]
        [TypeConverter(typeof(IndexFieldEnumerableConverter))]
        public virtual IEnumerable<ID> Categories { get; set; }

        [IndexField("keywords_sm")]
        [TypeConverter(typeof(IndexFieldEnumerableConverter))]
        public virtual IEnumerable<ID> Keywords { get; set; }

        public virtual Dictionary<string, string> Highlights { get; set; }

        public NewsBlogPostResultItem()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Highlights = new Dictionary<string, string>();
        }
    }
}