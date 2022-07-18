using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.StringExtensions;
using Sitecore.Workflows;

namespace AllinaHealth.Framework.Feeds
{
    public class ItemInWorkflow : Sitecore.Shell.Feeds.FeedTypes.Workflow
    {
        public string WorkflowItemDataUriKey = "dataUri";
        private Item _workflowItem;

        private Item WorkflowItem
        {
            get
            {
                if (_workflowItem != null)
                {
                    return _workflowItem;
                }

                if (!string.IsNullOrEmpty(Parameters[WorkflowItemDataUriKey]))
                {
                    _workflowItem = Client.ContentDatabase.GetItem(new DataUri(Parameters[WorkflowItemDataUriKey]));
                }

                return _workflowItem;
            }
        }

        public ItemInWorkflow(Item feedItem) : base(feedItem)
        {
            Assert.ArgumentNotNull(feedItem, "feedItem");
        }

        private IWorkflow GetWorkflow()
        {
            if (!Parameters.ContainsKey("wf"))
                throw new SyndicationException("Workflow feed expects the workflow parameter that was not supplied, the feed url is likely to be malformed");
            var workflowProvider = Client.ContentDatabase.WorkflowProvider;
            if (workflowProvider == null)
                throw new SyndicationException("Workflows are not enabled");
            var workflow = workflowProvider.GetWorkflow(Parameters["wf"]);
            if (workflow != null)
                return workflow;
            throw new SyndicationException("The {0} workflow doesn't exist any longer".FormatWith(Parameters["wf"]));
        }

        protected override IList<SyndicationItem> GetSyndicationItems()
        {
            var workflow = GetWorkflow();
            var list = new List<SyndicationItem>();

            if (WorkflowItem == null)
            {
                return list;
            }

            var history = workflow.GetHistory(WorkflowItem);
            if (history.Length == 0) return list;
            var workflowEvent = history.Last();
            list.Add(BuildSyndicationItem(WorkflowItem, workflowEvent));

            return list;
        }
    }
}