using System;
using Quartz;
using Sitecore.Diagnostics;
using Sitecron.SitecronSettings;

namespace AllinaHealth.Framework.SiteCron
{
    public abstract class SiteCronBase : IJob
    {
        private DateTime _lastLogEntry;
        protected abstract void Run(IJobExecutionContext args);

        public void Execute(IJobExecutionContext context)
        {
            var startExecution = DateTime.Now;
            _lastLogEntry = DateTime.Now;
            Run(context);
            _lastLogEntry = startExecution;
            WriteLogLine(context, "Job completed in elapsed time shown.");
        }

        protected void WriteLogLine(IJobExecutionContext context, string value)
        {
            var log = context.JobDetail.JobDataMap.GetString(SitecronConstants.ParamNames.SitecronJobLogData);

            var line = $"{DateTime.Now.ToUniversalTime()} - - {value}, Elapsed time since last step: {(DateTime.Now - _lastLogEntry).TotalSeconds} seconds";
            Log.Info(line, this);
            log = log + "\r\n" + line;

            context.JobDetail.JobDataMap.Put(SitecronConstants.ParamNames.SitecronJobLogData, log);
            _lastLogEntry = DateTime.Now;
        }
    }
}