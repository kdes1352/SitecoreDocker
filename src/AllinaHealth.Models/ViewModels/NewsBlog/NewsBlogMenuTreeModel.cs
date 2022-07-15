using System.Collections.Generic;
using AllinaHealth.Models.Extensions;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Links;

namespace AllinaHealth.Models.ViewModels.NewsBlog
{
    public class NewsBlogMenuTreeModel
    {
        public NewsBlogMenuTreeModel Parent { get; set; }
        public List<NewsBlogMenuTreeModel> Children { get; set; }
        public Item Item { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }

        public NewsBlogMenuTreeModel(Item item)
        {
            if (null != item)
            {
                FillModel(this, item, null, null, false);
            }
        }

        public NewsBlogMenuTreeModel(Item item, NewsBlogMenuTreeModel parent, bool loadChildern)
        {
            if (null != item)
            {
                FillModel(this, item, null, parent, loadChildern);
            }
        }

        public NewsBlogMenuTreeModel(Item item, string title, NewsBlogMenuTreeModel parent, bool loadChildern)
        {
            if (null != item)
            {
                FillModel(this, item, title, parent, loadChildern);
            }
        }

        public NewsBlogMenuTreeModel(Item item, bool loadChildern)
        {
            if (null != item)
                FillModel(this, item, null, null, loadChildern);
        }

        public NewsBlogMenuTreeModel()
        {
            Children = new List<NewsBlogMenuTreeModel>();
        }

        public NewsBlogMenuTreeModel(string title, string url, NewsBlogMenuTreeModel parent)
        {
            Title = title;
            URL = url;
            Parent = parent;
            Children = new List<NewsBlogMenuTreeModel>();
        }

        public void AddChild(Item child, bool loadChildern)
        {
            if (null == child) return;
            if (null == Children)
            {
                Children = new List<NewsBlogMenuTreeModel>();
            }

            var newChild = new NewsBlogMenuTreeModel(child, this, loadChildern);
            Children.Add(newChild);
        }

        public void AddChild(NewsBlogMenuTreeModel child)
        {
            if (null == child) return;
            if (null == Children)
            {
                Children = new List<NewsBlogMenuTreeModel>();
            }

            Children.Add(child);
        }

        private void BuildObj(Item item, string title, NewsBlogMenuTreeModel parent)
        {
            Item = item;
            Parent = parent;
            //m.URL = i.GetLinkFieldUrl("Menu Link");
            //m.Title = i.GetFieldValue("Menu Link Text");
            URL = LinkManager.GetItemUrl(item);
            Title = string.IsNullOrEmpty(title) ? item.Name : title;
            IsActive = false;
            IsSelected = false;

            if (item != null && item.ID == Context.Item.ID)
            {
                IsSelected = true;
                var current = Parent;
                while (current != null)
                {
                    current.IsActive = true;
                    current = current.Parent;
                }
            }

            Children = new List<NewsBlogMenuTreeModel>();
        }

        private void FillModel(NewsBlogMenuTreeModel m, Item i, string title, NewsBlogMenuTreeModel parent, bool loadChildern)
        {
            m.BuildObj(i, title, parent);
            //m.Item = i;
            //m.Parent = parent;
            //m.URL = Sitecore.Links.LinkManager.GetItemUrl(i);
            //m.Title = i.Name;
            //m.IsActive = false;
            //m.IsSelected = false;

            //var linkFieldTargetItem = i;
            //if (linkFieldTargetItem != null && linkFieldTargetItem.ID == Sitecore.Context.Item.ID)
            //{
            //    m.IsSelected = true;
            //    var current = m.Parent;
            //    while (current != null)
            //    {
            //        current.IsActive = true;
            //        current = current.Parent;
            //    }
            //}

            //m.Children = new List<NewsBlogMenuTreeModel>();
            if (!loadChildern) return;
            if (null == Children)
            {
                Children = new List<NewsBlogMenuTreeModel>();
            }

            foreach (var c in i.GetChildrenSafe())
            {
                var childModel = new NewsBlogMenuTreeModel();
                FillModel(childModel, c, null, m, false);
                m.Children.Add(childModel);
            }

        }
    }
}