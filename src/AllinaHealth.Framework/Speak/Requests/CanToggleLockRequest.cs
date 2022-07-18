using AllinaHealth.Framework.Extensions;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Context = Sitecore.Context;

namespace AllinaHealth.Framework.Speak.Requests
{
    public class CanToggleLockRequest : PipelineProcessorControlStateRequest<ItemContext>
    {
        public override bool GetControlState()
        {
            RequestContext.ValidateContextItem();
            return CanLock(RequestContext.Item);
        }

        private static bool CanLock(Item item)
        {
            Assert.ArgumentNotNull(item, nameof(item));

            if (!TemplateManager.IsFieldPartOfTemplate(FieldIDs.Workflow, item) | !TemplateManager.IsFieldPartOfTemplate(FieldIDs.WorkflowState, item)
                || !item.Access.CanWrite()
                || (!item.Access.CanReadLanguage()
                    || !item.Access.CanWriteLanguage())
                || (!Context.IsAdministrator || !Context.User.IsUserBusinessAnalystRole())
                && item.Locking.IsLocked()
                && !item.Locking.HasLock()
                || !(item.Locking.CanLock() | item.Locking.CanUnlock()))
            {
                return false;
            }

            return !item.Appearance.ReadOnly;
        }
    }
}