using System;
using AllinaHealth.Framework.SiteCron;
using Quartz;
using Sitecore.ContentSearch;

namespace AllinaHealth.Framework.Tasks
{
    class SiteCronRebuildSearchIndex : SiteCronBase
    {
        protected override void Run(IJobExecutionContext context)
        {
            try
            {
                var startTime = DateTime.Now;
                var mName = Environment.MachineName;
                WriteLogLine(context, $"SEARCH INDEX: Starting from SiteCron task on {mName}");

                var index = ContentSearchManager.GetIndex("search_index");
                index.Rebuild();

                var interval = DateTime.Now - startTime;
                var hours = Math.Floor(interval.TotalHours);
                WriteLogLine(context, $"SEARCH INDEX: Finished from SiteCron task in {hours} hours {interval.TotalMinutes} minutes on {mName}");
            }
            catch (Exception e)
            {
                WriteLogLine(context, "Error Rebuilding Search Index:");
                WriteLogLine(context, $"Message: {e.Message}, Inner Exception Message: {e.InnerException?.Message}");
            }
        }
    }
}