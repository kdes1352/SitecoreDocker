using System.Collections.Generic;
using Sitecore.Data.Items;

namespace AllinaHealth.Models.ViewModels.NewsBlog
{
    public class HomePageViewModel
    {
        private Item _newsBlogItem;
        private readonly List<Item> _list;
        private NewsBlogMenuTreeModel _menu;

        public HomePageViewModel()
        {
            _newsBlogItem = null;
            _list = new List<Item>();
        }

        public HomePageViewModel(Item model, List<Item> topResults, NewsBlogMenuTreeModel menuTree)
        {
            _newsBlogItem = model;
            _list = topResults;
            _menu = menuTree;
        }

        // ReSharper disable once ConvertToAutoProperty
        public Item NewsBlogPostItem
        {
            get => _newsBlogItem;
            set => _newsBlogItem = value;
        }

        public List<Item> TopPosts => _list ?? new List<Item>();

        public void SetTopPosts(List<Item> items)
        {
            if (null == items || items.Count <= 0) return;
            _list.Clear();
            _list.AddRange(items);
        }

        // ReSharper disable once ConvertToAutoProperty
        public NewsBlogMenuTreeModel MenuTree
        {
            get => _menu;
            set => _menu = value;
        }

        public int Page { get; set; }

        public bool HasMoreResults { get; set; }
    }
}