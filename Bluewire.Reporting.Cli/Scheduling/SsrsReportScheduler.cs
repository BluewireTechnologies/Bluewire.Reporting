using Bluewire.Reporting.Cli.Model;
using Bluewire.Reporting.Common.ServiceProxy;
using Task = System.Threading.Tasks.Task;

namespace Bluewire.Reporting.Cli.Scheduling
{
    public class SsrsReportScheduler
    {
        private readonly IReportingServiceClient service;
        private readonly ISsrsReportSchedulingRule rule;

        public SsrsReportScheduler(IReportingServiceClient service, ISsrsReportSchedulingRule rule)
        {
            this.service = service;
            this.rule = rule;
        }

        public Task Apply(SsrsReport report)
        {
            return rule.Apply(service, report);
        }
    }
}
