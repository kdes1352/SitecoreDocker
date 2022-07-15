using System.Collections.Generic;
using AllinaHealth.Models.Extensions;

namespace AllinaHealth.Models.ViewModels.HSG
{
    public class NavigationViewModel
    {
        private readonly List<CategoryViewModel> _list = new List<CategoryViewModel>();

        public NavigationViewModel()
        {
            var hsgHome = Sitecore.Context.Item.GetFirstParentOfTemplate("{89EC0E5B-AD89-4F5F-90C9-4E62E3559F1F}");
            if (hsgHome == null) return;
            foreach (var i in hsgHome.GetChildrenSafe())
            {
                _list.Add(new CategoryViewModel(i));
            }
        }

        public List<CategoryViewModel> List => _list;
    }
}
