using Bluewire.Reporting.Cli.Model;
using Bluewire.Reporting.Common.ServiceProxy;
using Task = System.Threading.Tasks.Task;
using Bluewire.Reporting.Common.Scheduling;
using log4net;

namespace Bluewire.Reporting.Cli.Scheduling
{
    public class SsrsInferredSchedulingRule : ISsrsReportSchedulingRule
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SsrsInferredSchedulingRule));

        public async Task Apply(IReportingServiceClient service, SsrsReport report)
        {
            var reportDto = await new ReportForSchedulingReader().FromSsrs(service, report.Path);
            var rules = new EproSchedulingRules().Interpret(reportDto);
            log.Debug($"  Frequency: {rules.Frequency}");
            log.Debug($"     Period: {rules.Period}");

            var request = new SetItemHistoryOptionsRequest {
                ItemPath = report.Path,
                EnableManualSnapshotCreation = true,
                KeepExecutionSnapshots = false,
                Item = new EproSchedulingRules().CreateScheduleDefinition(rules)
            };
            await service.Proxy.SetItemHistoryOptionsAsync(request);
        }
    }
}
