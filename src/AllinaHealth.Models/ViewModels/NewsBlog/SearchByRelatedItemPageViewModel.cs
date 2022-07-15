﻿using System.Collections.Generic;
using Sitecore.Data.Items;

namespace AllinaHealth.Models.ViewModels.NewsBlog
{
    public class SearchByRelatedItemPageViewModel
    {
        private Item _item;
        private readonly List<Item> _list;
        private NewsBlogMenuTreeModel _menu;

        // ReSharper disable once InconsistentNaming
        public string SearchItemID { get; set; }

        public SearchByRelatedItemPageViewModel()
        {
            _item = null;
            _list = new List<Item>();
        }

        public SearchByRelatedItemPageViewModel(Item model)
        {
            _item = model;
            _list = new List<Item>();
        }

        // ReSharper disable once ConvertToAutoProperty
        public Item NewsItem
        {
            get => _item;
            set => _item = value;
        }

        public List<Item> Posts => _list ?? new List<Item>();

        public void SetPosts(List<Item> items)
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