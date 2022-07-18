using System;
using System.Collections.Generic;
using System.Linq;
using AllinaHealth.Models.Global.Folders;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pipelines.GetRenderingDatasource;
using Sitecore.SecurityModel;

namespace AllinaHealth.Framework.Pipelines.GetRenderingDatasource
{
    public class GetSiteRootDatasourceLocations : GetDatasourceAbstractProcessor<GetRenderingDatasourceArgs>
    {
        public const string Token = "$siteroot";

        public override Item[] GetDatasourceRoots(string value)
        {
            return GetSourcesByToken(value, Token);
        }

        public override string SetPropertiesAndGetSourceValue(GetRenderingDatasourceArgs args)
        {
            ContentDatabase = args.ContentDatabase;
            ContextItemPath = args.ContextItemPath;

            return args.RenderingItem["Datasource Location"];
        }

        public override void AddDatasourceRoots(GetRenderingDatasourceArgs args, Item[] list, string sourceValue)
        {
            //GetGlobalDataRoot(args, list);
            GetPageDataRoot(args);
            args.DatasourceRoots.AddRange(list);
        }

        // ReSharper disable once UnusedMember.Local
        private void GetGlobalDataRoot(GetRenderingDatasourceArgs args, IEnumerable<Item> list)
        {
            var sharedSiteComponents = ContentDatabase.GetItem("/sitecore/content/data");
            if (sharedSiteComponents == null)
            {
                return;
            }

            foreach (var i in list)
            {
                var dsLocation = sharedSiteComponents.GetChildren().FirstOrDefault(e => e.Name == i.Name);
                if (dsLocation != null)
                {
                    args.DatasourceRoots.Add(dsLocation);
                }
                else
                {
                    var newFolder = i.CopyTo(sharedSiteComponents, i.Name, ID.NewID, false);
                    using (new SecurityDisabler())
                    {
                        using (new EditContext(newFolder))
                        {
                            newFolder.Fields[FieldIDs.DisplayName].SetValue("Global " + newFolder.Name, true);
                        }
                    }

                    args.DatasourceRoots.Add(newFolder);
                }
            }
        }

        private void GetPageDataRoot(GetRenderingDatasourceArgs args)
        {
            if (ContextItem == null)
            {
                return;
            }

            var dsLocation = ContextItem.GetChildren().FirstOrDefault(e => e.Name.Equals("data", StringComparison.InvariantCultureIgnoreCase));
            if (dsLocation != null)
            {
                args.DatasourceRoots.Add(dsLocation);
            }
            else
            {
                var dataFolderTemplate = ContentDatabase.GetTemplate(IData_FolderConstants.TemplateId);
                var newFolder = ContextItem.Add("Data", dataFolderTemplate);
                {
                    using (new SecurityDisabler())
                    {
                        using (new EditContext(newFolder))
                        {
                            newFolder.Fields[FieldIDs.DisplayName].SetValue("Page Data", true);
                            newFolder.Fields[FieldIDs.Sortorder].SetValue("1", true);
                        }
                    }
                }
                args.DatasourceRoots.Add(newFolder);
            }
        }
    }
}