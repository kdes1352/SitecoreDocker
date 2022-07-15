using Sitecore.Data.Items;

namespace AllinaHealth.Models.ViewModels.NewsBlog
{
    public class PostViewModel
    {
        private readonly Item _newsBlogItem;
        private readonly NewsBlogMenuTreeModel _menu;

        public PostViewModel(Item model, NewsBlogMenuTreeModel menuTree)
        {
            _newsBlogItem = model;
            _menu = menuTree;
        }

        // ReSharper disable once ConvertToAutoProperty
        public Item NewsBlogItem => _newsBlogItem;

        // ReSharper disable once ConvertToAutoProperty
        public NewsBlogMenuTreeModel MenuTree => _menu;

        public string PhotoWidth { get; set; }
    }
}