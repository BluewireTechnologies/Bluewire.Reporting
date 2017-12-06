using Bluewire.Reporting.Cli.Model;
using Bluewire.Reporting.Common.ServiceProxy;
using Task = System.Threading.Tasks.Task;

namespace Bluewire.Reporting.Cli.Scheduling
{
    public interface ISsrsReportSchedulingRule
    {
        Task Apply(IReportingServiceClient service, SsrsReport report);
    }
}
