using System.Collections.Generic;
using AllinaHealth.Models.Extensions;
using Sitecore.Data.Items;

namespace AllinaHealth.Models.ViewModels.Menus
{
    public class MenuTreeModel
    {
        public MenuTreeModel Parent { get; set; }
        public List<MenuTreeModel> Children { get; set; }
        public Item Item { get; set; }
        // ReSharper disable once InconsistentNaming
        public string URL { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }

        public MenuTreeModel(Item menuRootItem)
        {
            FillModel(this, menuRootItem, null);
        }

        private MenuTreeModel()
        {

        }

        private static void FillModel(MenuTreeModel m, Item i, MenuTreeModel parent)
        {
            m.Item = i;
            m.Parent = parent;
            m.URL = i.GetLinkFieldUrl("Menu Link");
            m.Title = i.GetFieldValue("Menu Link Text");
            m.IsActive = false;
            m.IsSelected = false;

            var linkFieldTargetItem = i.GetLinkFieldTargetItem("Menu Link");
            if (linkFieldTargetItem != null && linkFieldTargetItem.ID == Sitecore.Context.Item.ID)
            {
                m.IsSelected = true;
                var current = m.Parent;
                while (current != null)
                {
                    current.IsActive = true;
                    current = current.Parent;
                }
            }

            m.Children = new List<MenuTreeModel>();
            foreach (var c in i.GetChildrenSafe())
            {
                var childModel = new MenuTreeModel();
                FillModel(childModel, c, m);
                m.Children.Add(childModel);
            }
        }
    }
}