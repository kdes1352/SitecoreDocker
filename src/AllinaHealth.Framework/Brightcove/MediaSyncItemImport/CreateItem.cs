using System;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.MediaFramework.Diagnostics;

namespace AllinaHealth.Framework.Brightcove.MediaSyncItemImport
{
    public class CreateItem : Sitecore.MediaFramework.Pipelines.MediaSyncImport.MediaSyncItemImport.CreateItem
    {
        protected override Item Create(string itemName, ID templateId, Item rootItem, ID itemId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(itemName))
                {
                    itemName = itemName.Trim();
                    if (!string.IsNullOrWhiteSpace(itemName))
                    {
                        if (char.IsDigit(itemName[0]))
                        {
                            itemName = itemName.Replace(" ", "_");
                        }

                        itemName = ItemUtil.ProposeValidItemName(itemName);
                        return ItemManager.AddFromTemplate(itemName, templateId, rootItem, itemId);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error during item creation. Item name: \"{itemName}\"", this, ex);
            }

            return null;
        }
    }
}