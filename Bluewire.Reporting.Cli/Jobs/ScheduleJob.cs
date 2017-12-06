using System;
using System.IO;
using System.Linq;
using Bluewire.Reporting.Cli.Model;
using Bluewire.Reporting.Cli.Scheduling;
using Bluewire.Reporting.Cli.Sources;
using log4net;
using Task = System.Threading.Tasks.Task;

namespace Bluewire.Reporting.Cli.Jobs
{
    public class ScheduleJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ScheduleJob));
        private readonly ISsrsObjectSource source;
        private readonly SsrsReportScheduler scheduler;
        private readonly SsrsObjectFilter filter;

        public ScheduleJob(
            ISsrsObjectSource source,
            SsrsReportScheduler scheduler,
            SsrsObjectFilter filter
            )
        {
            this.source = source;
            this.scheduler = scheduler;
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public async Task Run(TextWriter output)
        {
            log.Info("Collecting objects...");
            var reports = (await source.Enumerate(filter)).OfType<SsrsReport>().ToArray();

            foreach (var report in reports)
            {
                log.DebugFormat("Scheduling report: '{0}'", report.Path);
                await scheduler.Apply(report);
            }

            log.Info("Done.");
        }
    }
}
