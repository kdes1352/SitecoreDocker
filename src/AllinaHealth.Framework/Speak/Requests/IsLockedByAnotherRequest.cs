using AllinaHealth.Framework.Extensions;
using AllinaHealth.Framework.Pipelines.GetContentEditorWarnings;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Globalization;
using Context = Sitecore.Context;

namespace AllinaHealth.Framework.Speak.Requests
{
    public class IsLockedByAnotherRequest : PipelineProcessorRequest<ItemContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            RequestContext.ValidateContextItem();
            var processorResponseValue = new PipelineProcessorResponseValue();
            var obj = RequestContext.Item;
            if (Context.User.IsAdministrator || Context.User.IsUserBusinessAnalystRole())
            {
                return processorResponseValue;
            }

            if (obj.Locking.IsLocked() && !obj.Locking.HasLock())
            {
                processorResponseValue.AbortMessage = Translate.Text("<b>\"{0}\"</b> has locked this item.", IsLocked.GetUserName(RequestContext.Item));
            }

            return processorResponseValue;
        }
    }
}