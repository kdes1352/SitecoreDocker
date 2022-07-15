using AllinaHealth.Models.Extensions;
using Sitecore.Data.Items;

namespace AllinaHealth.Models.Contexts
{
    public class SiteContextModel
    {
        private Item _siteFolder;
        private Item _homeItem;
        private Item _page404;
        private Item _page500;

        public SiteContextModel()
        {

        }

        public Item DataSourceItem => Sitecore.Mvc.Presentation.RenderingContext.Current.Rendering.Item;

        public Item SiteFolder
        {
            get
            {
                if (_siteFolder != null) return _siteFolder;
                _siteFolder = GetSiteRootFolder(Sitecore.Context.Item);
                if (_siteFolder != null) return _siteFolder;
                _homeItem = Sitecore.Context.Database.GetItem(Sitecore.Context.Site.StartPath);
                _siteFolder = _homeItem.Parent;
                return _siteFolder;
            }
        }

        public Item HomeItem
        {
            get
            {
                if (_homeItem != null) return _homeItem;
                _homeItem = SiteFolder.GetFirstChildWithTemplate("{E7B733C0-C627-45CF-82D3-0F7EEC157514}");
                if (_homeItem != null) return _homeItem;
                _homeItem = Sitecore.Context.Database.GetItem(Sitecore.Context.Site.StartPath);
                _siteFolder = _homeItem.Parent;
                return _homeItem;
            }
        }

        public Item Page500 =>
            _page500 ?? (_page500 =
                HomeItem.GetFirstChildWithBaseTemplate("{A09464BA-9DE6-4EE1-9B8E-A49671444F72}"));

        public Item Page404 =>
            _page404 ?? (_page404 =
                HomeItem.GetFirstChildWithBaseTemplate("{66FB8118-F4F3-43AF-B230-D81ECD4AB180}"));

        public static Item GetSiteRootFolder(Item i)
        {
            if (i != null)
            {
                return i.GetFirstParentOfTemplate("{7B30C1AA-838C-4357-BDB7-19D17129A74D}");
            }

            if (Sitecore.Context.Site == null) return null;
            var homeItem = Sitecore.Context.Database.GetItem(Sitecore.Context.Site.RootPath);
            return homeItem?.Parent;
        }

        public string HeaderEnvironment => Sitecore.Configuration.Settings.GetSetting("Header.Environment", "local");

        public string WellclicksHostName => Sitecore.Configuration.Settings.GetSetting("Wellclicks.HostName", "contentdev.wellclicks.com");

        public string CmsClass => string.Empty;

        public string GetDictionaryText(string key)
        {
            return Sitecore.Globalization.Translate.TextByDomain(Sitecore.Context.Site.DictionaryDomain, key);
        }

        public string CmsClassRequired => Sitecore.Configuration.Settings.GetSetting("Cms.Class.Required", "");
    }
}
