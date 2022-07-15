using Sitecore.ContentSearch;

namespace AllinaHealth.Models.ContentSearch
{
    public class NewsBlogCategoryResultItem : BaseSearchResultItem
    {
        [IndexField("show_in_menu_b")]
        public virtual bool ShowInMenu { get; set; }

        [IndexField("category_t")]
        public virtual string Category { get; set; }

        [IndexField("sortablecategory_s")]
        public virtual string CategorySort { get; set; }
    }
}