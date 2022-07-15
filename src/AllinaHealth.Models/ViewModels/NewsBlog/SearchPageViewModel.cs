using System.Collections.Generic;
using AllinaHealth.Models.ContentSearch;
using AllinaHealth.Models.ViewModels.Search;

namespace AllinaHealth.Models.ViewModels.NewsBlog
{
    public class SearchPageViewModel
    {
        public string SearchText { get; set; }
        private List<NewsBlogPostResultItem> _list;
        private NewsBlogMenuTreeModel _menu;
        public int TotalHits { get; set; }
        public int Page { get; set; }
        public Pager Pager { get; private set; }

        public SearchPageViewModel() : this(25)
        {
        }

        // ReSharper disable once UnusedParameter.Local
        public SearchPageViewModel(int pageSize, string searchUrl = "")
        {
            _list = new List<NewsBlogPostResultItem>();
            Pager = new Pager(0, null, pageSize);
            _menu = new NewsBlogMenuTreeModel();
        }

        //public SearchPageViewModel(string SearchText, List<NewsBlogPostResultItem> Results, int TotalHits, int PageSize, int CurrentPage)
        //{
        //    this.SearchText = SearchText;
        //    _list = Results;
        //    this.TotalHits = TotalHits;
        //    Pager = new Pager(TotalHits, CurrentPage, PageSize);
        //}
        public List<NewsBlogPostResultItem> Posts
        {
            get => _list ?? new List<NewsBlogPostResultItem>();
            set => _list = value;
        }

        public void UpdatePager(int totalHits, int pageSize, int currentPage)
        {
            Pager = new Pager(totalHits, currentPage, pageSize);
        }

        // ReSharper disable once ConvertToAutoProperty
        public NewsBlogMenuTreeModel MenuTree
        {
            get => _menu;
            set => _menu = value;
        }
    }
}