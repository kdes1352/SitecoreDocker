using System.Collections.Generic;
using System.ComponentModel;
using AllinaHealth.Models.ContentSearch;
using Sitecore.Mvc.Presentation;

namespace AllinaHealth.Models.ViewModels.Search
{
    public class SearchPage : RenderingModel
    {
        [DisplayName("Site Search")] public string SearchText { get; set; }

        public List<SiteSearchPreferredResultItem> PreferredResults { get; set; }
        public List<SiteSearchResultItem> Results { get; set; }

        public int TotalHits { get; set; }

        public Pager Pager { get; }

        public SearchPage() : this(25)
        {
            //Some default value;
        }

        public SearchPage(int pageSize)
        {
            SearchText = "";
            Results = new List<SiteSearchResultItem>();
            PreferredResults = new List<SiteSearchPreferredResultItem>();
            Pager = new Pager(0, null, pageSize);
        }

        public SearchPage(string searchText, List<SiteSearchResultItem> results, List<SiteSearchPreferredResultItem> preferredResults, int totalHits, int pageSize, int currentPage)
        {
            SearchText = searchText;
            Results = results;
            PreferredResults = preferredResults;
            TotalHits = totalHits;
            Pager = new Pager(totalHits, currentPage, pageSize);
        }
    }
}