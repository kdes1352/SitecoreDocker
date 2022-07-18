using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel.Syndication;
using System.Web;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Shell.Feeds;
using Sitecore.Workflows;
using Sitecore.Workflows.Simple;

namespace AllinaHealth.Framework.Workflow.Actions
{
    public class EmailAction
    {
        private Item _currentItem;
        private ProcessorItem _currentProcessorItem;
        private IWorkflow _currentWorkflow;
        private const string ExecuteEmailActionKey = "Execute Eamil Action";

        private Item CurrentItem => _currentItem;

        private ProcessorItem CurrentProcessorItem => _currentProcessorItem;

        private IWorkflow CurrentWorkflow
        {
            get
            {
                if (_currentWorkflow == null && CurrentItem != null)
                {
                    _currentWorkflow = CurrentItem.Database.WorkflowProvider.GetWorkflow(CurrentItem);
                }

                return _currentWorkflow;
            }
        }

        public string WorkflowEmailTo => Settings.GetSetting("Workflow.EmailTo", "news@allina.com");

        // ReSharper disable once UnusedMember.Global
        public void Process(WorkflowPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (Settings.GetSetting("InstanceName", string.Empty).ToLower().Trim() != "dx-cm-prd")
            {
                return;
            }

            _currentItem = args.DataItem;
            _currentProcessorItem = args.ProcessorItem;
            if (CurrentItem != null && RunEmailActionAfterStateChange(args) && CurrentProcessorItem != null)
            {
                SendMessage();
            }
        }

        public bool RunEmailActionAfterStateChange(WorkflowPipelineArgs args)
        {
            if (!args.CustomData.ContainsKey(ExecuteEmailActionKey))
            {
                args.CustomData.Add(ExecuteEmailActionKey, CurrentProcessorItem);
                var emailActionProcessor = new Processor("emailActionProcessor", this, "Process");
                args.Pipeline.Add(emailActionProcessor);
                return false;
            }

            _currentProcessorItem = args.CustomData[ExecuteEmailActionKey] as ProcessorItem;
            return true;
        }

        private string GetFeedMessage()
        {
            var clientFeed = ClientFeedManager.GetFeed("ItemInWorkflow");
            var parameters = new Dictionary<string, string>
            {
                { "wf", CurrentWorkflow.WorkflowID },
                { "dataUri", CurrentItem.Uri.ToDataUri().ToString() }
            };
            clientFeed.SetParameters(parameters);

            try
            {
                var synFeed = clientFeed.Render();
                var currentItemIdEncoded = HttpUtility.UrlEncode(CurrentItem.ID.ToString()).ToUpper();
                var synItem = synFeed.Items.FirstOrDefault(e => e.Id.Contains(currentItemIdEncoded));
                var tsc = (TextSyndicationContent)synItem?.Content;
                if (tsc != null)
                {
                    return "<html><body>" + tsc.Text + "</body></html>";
                }
            }
            catch
            {
                // ignored
            }

            return string.Empty;
        }

        private string GetSubject()
        {
            var state = CurrentProcessorItem.InnerItem.Parent;
            return $"{CurrentWorkflow.Appearance.DisplayName} - {state.DisplayName}: {CurrentItem.Paths.FullPath}";
        }

        private void SendMessage()
        {
            var subject = GetSubject();
            var body = GetFeedMessage();

            if (string.IsNullOrEmpty(body))
            {
                return;
            }

            try
            {
                var emailMessage = new MailMessage("allina.webmaster@allina.com", WorkflowEmailTo, subject, body);
                emailMessage.IsBodyHtml = true;
                MainUtil.SendMail(emailMessage);
            }
            catch (Exception ex)
            {
                Log.Error("EmailAction.SendMessage(MailMessage message)", ex, this);
            }
        }
    }
}