using System;
using Sitecore.Diagnostics;

namespace AllinaHealth.Framework.Events
{
    public class DictionaryCacheClearer
    {
        public void ClearCache(object sender, EventArgs args)
        {
            Sitecore.Globalization.Translate.ResetCache();
            Log.Info("Dictionary Cache Cleared", this);
        }
    }
}