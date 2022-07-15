using System;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;

namespace AllinaHealth.Framework.Commands
{
    [Serializable]
    public class CustomReportsCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Windows.RunApplication("/sitecore/content/Applications/Tools/Custom Reports");
        }
    }
}