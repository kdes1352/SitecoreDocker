using System;
using System.Collections.Generic;
using AllinaHealth.Framework.Extensions;
using AllinaHealth.Models.Contexts;
using AllinaHealth.Models.Extensions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Text;

namespace AllinaHealth.Framework.Pipelines.GetRenderingDatasource
{
    public abstract class GetDatasourceAbstractProcessor<T> where T : PipelineArgs
    {
        private string _contextItemPath;
        private Item _contextItem;

        #region Properties

        public Database ContentDatabase { get; set; }

        public Item ContextItem
        {
            get => _contextItem ?? (_contextItem = ContentDatabase.GetItem(ContextItemPath));
            set => _contextItem = value;
        }

        public string ContextItemPath
        {
            get => _contextItemPath;
            set
            {
                _contextItemPath = value;
                _contextItem = null;
            }
        }

        public bool IsTemplateItemPath => ContextItem.Paths.FullPath.ToLower().StartsWith("/sitecore/templates/");

        public Item RenderingItem { get; set; }

        #endregion

        #region Methods

        public abstract Item[] GetDatasourceRoots(string value);

        public abstract string SetPropertiesAndGetSourceValue(T args);

        public abstract void AddDatasourceRoots(T args, Item[] list, string sourceValue);

        public void Process(T args)
        {
            Assert.IsNotNull(args, "args");
            foreach (var value in new ListString(SetPropertiesAndGetSourceValue(args)))
            {
                AddDatasourceRoots(args, GetDatasourceRoots(value), value);
            }
        }

        public Item[] GetSiteRootItems()
        {
            var list = new List<Item>();
            const string siteRootIdString = "{7B30C1AA-838C-4357-BDB7-19D17129A74D}";
            var siteRootTemplate = ContentDatabase.GetItem(new ID(siteRootIdString));
            if (siteRootTemplate == null) return list.ToArray();
            foreach (var templateItem in siteRootTemplate.GetReferrers())
            {
                list.AddRange(templateItem.GetReferrers());
            }

            return list.ToArray();
        }

        public Item[] GetSourcesByToken(string value, string token)
        {
            var list = new List<Item>();

            if (!value.StartsWith(token, StringComparison.InvariantCulture)) return list.ToArray();

            if (IsTemplateItemPath)
            {
                foreach (var siteRoot in GetSiteRootItems())
                {
                    list.AddNonNull(ContentDatabase.GetItem(value.Replace(token, siteRoot.Paths.FullPath)));
                }
            }
            else
            {
                var siteRoot = SiteContextModel.GetSiteRootFolder(ContextItem);

                if (siteRoot != null)
                {
                    list.AddNonNull(ContentDatabase.GetItem(value.Replace(token, siteRoot.Paths.FullPath)));
                }
            }

            return list.ToArray();
        }

        #endregion

    }
}