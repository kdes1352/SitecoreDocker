using System;
using System.Collections.Specialized;
using AllinaHealth.Framework.Extensions;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Version = Sitecore.Data.Version;

namespace AllinaHealth.Framework.Commands
{
    [Serializable]
    public class UnlockItem : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));
            if (context.Items.Length != 1)
            {
                return;
            }

            var obj = context.Items[0];
            Context.ClientPage.Start(this, "Run", new NameValueCollection
            {
                ["id"] = obj.ID.ToString(),
                ["language"] = obj.Language.ToString(),
                ["version"] = obj.Version.ToString()
            });
        }

        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            var itemNotNull = Client.GetItemNotNull(args.Parameters["id"], Language.Parse(args.Parameters["language"]), Version.Parse(args.Parameters["version"]));
            if (!itemNotNull.Locking.IsLocked())
            {
                return;
            }

            using (new SecurityDisabler())
            {
                using (new EditContext(itemNotNull))
                {
                    itemNotNull.Locking.Unlock();
                }
            }

            Context.ClientPage.SendMessage(this, "item:checkedin");
        }

        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            if (!Context.User.IsUserBusinessAnalystRole() && !Context.IsAdministrator)
            {
                return CommandState.Hidden;
            }

            if (context.Items.Length != 1)
            {
                return CommandState.Hidden;
            }

            var obj = context.Items[0];
            if (Context.IsAdministrator)
            {
                return !obj.Locking.IsLocked() ? CommandState.Hidden : CommandState.Enabled;
            }

            if (obj.Appearance.ReadOnly || !obj.Access.CanWrite() || !obj.Access.CanWriteLanguage() || !obj.Locking.IsLocked())
            {
                return CommandState.Hidden;
            }

            return base.QueryState(context);

        }
    }
}