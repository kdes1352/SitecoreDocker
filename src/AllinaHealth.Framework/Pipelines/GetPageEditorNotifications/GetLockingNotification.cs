using AllinaHealth.Framework.Pipelines.GetContentEditorWarnings;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Utils;
using Sitecore.Globalization;
using Sitecore.Pipelines.GetPageEditorNotifications;

namespace AllinaHealth.Framework.Pipelines.GetPageEditorNotifications
{
    public class GetLockingNotification : GetPageEditorNotificationsProcessor
    {
        public override void Process(GetPageEditorNotificationsArgs arguments)
        {
            Assert.ArgumentNotNull(arguments, "The arguments is null.");

            var contextItem = arguments.ContextItem;
            if (contextItem == null)
            {
                return;
            }

            using (new LanguageSwitcher(WebUtility.ClientLanguage))
            {
                if (Context.User.IsAdministrator)
                {
                    if (!contextItem.Locking.IsLocked() || contextItem.Locking.HasLock())
                    {
                        return;
                    }

                    arguments.Notifications.Add(new PageEditorNotification(Translate.Text("'{0}' has locked this item.", IsLocked.GetUserName(contextItem)), PageEditorNotificationType.Warning));
                }
                else
                {
                    if (!contextItem.Locking.IsLocked() || contextItem.Locking.HasLock())
                    {
                        return;
                    }

                    arguments.Notifications.Add(new PageEditorNotification(Translate.Text("You cannot edit this item because '{0}' has locked it.", IsLocked.GetUserName(contextItem)), PageEditorNotificationType.Warning));
                }
            }
        }
    }
}