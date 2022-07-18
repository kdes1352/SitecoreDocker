using System;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Pipelines.GetContentEditorWarnings;
using Sitecore.Security.Accounts;

namespace AllinaHealth.Framework.Pipelines.GetContentEditorWarnings
{
    public class IsLocked
    {
        public void Process(GetContentEditorWarningsArgs args)
        {
            var obj = args.Item;
            if (obj == null)
                return;
            if (Sitecore.Context.IsAdministrator)
            {
                if (!obj.Locking.IsLocked() || string.Compare(obj.Locking.GetOwner(), Sitecore.Context.User.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return;
                args.Add(Translate.Text("'{0}' has locked this item.", GetUserName(obj)), string.Empty);
            }
            else if (obj.Locking.IsLocked())
            {
                if (obj.Locking.HasLock())
                    return;
                args.Add(Translate.Text("You cannot edit this item because '{0}' has locked it.", GetUserName(obj)), string.Empty);
            }
            else
            {
                if (!Settings.RequireLockBeforeEditing || !TemplateManager.IsFieldPartOfTemplate(Sitecore.FieldIDs.Lock, obj))
                    return;
                var contentEditorWarning = args.Add();
                contentEditorWarning.Title = Translate.Text("You must lock this item before you can edit it.");
                contentEditorWarning.Text = Translate.Text("To lock this item, click Edit on the Home tab.");
                contentEditorWarning.AddOption(Translate.Text("Lock and Edit"), "item:checkout");
            }
        }

        public static string GetUserName(Item i)
        {
            var lockUser = i.Locking.GetOwner();

            if (string.IsNullOrEmpty(lockUser))
            {
                return "CAN'T FIND USER";
            }

            var name = GetUserName(lockUser);
            return !string.IsNullOrEmpty(name) ? name : i.Locking.GetOwnerWithoutDomain();
        }

        public static string GetUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return string.Empty;
            }

            var u = User.FromName(userName, false);
            if (u == null)
            {
                return string.Empty;
            }

            var email = u.Profile.Email;
            if (string.IsNullOrEmpty(email))
            {
                return string.Empty;
            }

            var pos = email.IndexOf("@", StringComparison.Ordinal);
            if (pos <= 0)
            {
                return string.Empty;
            }

            email = email.Substring(0, pos);
            email = email.Replace(".", " ");
            return string.Format("{0} ({1})", email, u.LocalName);

        }
    }
}