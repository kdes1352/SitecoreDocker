using System.Collections.Generic;
using AllinaHealth.Models.Extensions;
using Sitecore.Data.Items;

namespace AllinaHealth.Models.ViewModels.Toolbox
{
    public class QandATreeModel
    {
        public QandATreeModel Parent { get; set; }
        public List<QandATreeModel> Children { get; set; }
        public List<QandATreeModel> TmpChildren { get; set; }
        public Item Item { get; set; }
        public List<Item> ImportedItems { get; set; }
        public Item ParentItem { get; set; }
        public string Answer { get; set; }
        public string Question { get; set; }

        // ReSharper disable once InconsistentNaming
        public string ItemID { get; set; }

        // ReSharper disable once InconsistentNaming
        public int cCounter;

        public QandATreeModel(Item qnaRootItem)
        {
            FillModel(this, qnaRootItem, null);
        }

        private QandATreeModel()
        {

        }

        private void FillModel(QandATreeModel m, Item i, QandATreeModel parent)
        {
            m.Item = i;
            m.Parent = parent;

            m.Answer = i.GetFieldValue("Answer Option");
            m.Question = i.GetFieldValue("Content Or Question");
            m.ImportedItems = i.GetSelectedItems("External Answers");

            cCounter++;
            var tmpItemId = "Item-" + cCounter + "-ID:" + i.ID.ToString().Replace("{", "").Replace("}", "");
            m.ItemID = tmpItemId;

            m.Children = new List<QandATreeModel>();

            //For "External Answers"
            foreach (var ii in m.ImportedItems)
            {
                var importedModel = new QandATreeModel();
                FillModel(importedModel, ii, m);
                m.Children.Add(importedModel);
            }

            foreach (var c in i.GetChildrenSafe())
            {
                var childModel = new QandATreeModel();
                FillModel(childModel, c, m);
                m.Children.Add(childModel);
            }
        }
    }
}