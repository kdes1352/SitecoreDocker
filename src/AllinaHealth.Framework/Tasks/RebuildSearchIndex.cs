using System;
using Sitecore.ContentSearch;
using Sitecore.Diagnostics;

namespace AllinaHealth.Framework.Tasks
{
    public class RebuildSearchIndex
    {
        public void Run()
        {
            try
            {
                var startTime = DateTime.Now;
                var mName = Environment.MachineName;
                Log.Warn($"SEARCH INDEX: Starting from schedulied task on {mName}", this);

                var index = ContentSearchManager.GetIndex("search_index");
                index.Rebuild();

                var interval = DateTime.Now - startTime;
                var hours = Math.Floor(interval.TotalHours);
                Log.Warn($"SEARCH INDEX: Finished from schedulied task in {hours} hours {interval.TotalMinutes} minutes on {mName}", this);
            }
            catch (Exception e)
            {
                Log.Error("Error Rebuilding Search Index", e, this);
            }
        }
    }
}