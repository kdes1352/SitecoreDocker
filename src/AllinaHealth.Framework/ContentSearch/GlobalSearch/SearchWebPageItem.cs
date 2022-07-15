namespace AllinaHealth.Framework.ContentSearch.GlobalSearch
{
    public class SearchWebPageItem
    {
        public string Url { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }

        // ReSharper disable once InconsistentNaming
        public string SitecoreID { get; set; }
        public string SitecorePath { get; set; }
        public string MetaDescription { get; set; }
        public bool PreferredSearchItem { get; set; }
        public string PreferredKeywords { get; set; }
        public string OverrideTitle { get; set; }
        public string OverrideDescription { get; set; }
    }
}