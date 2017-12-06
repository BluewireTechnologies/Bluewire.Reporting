using System;
using System.Collections.Generic;
using System.Linq;
using Bluewire.Common.Console;
using Bluewire.Common.Console.ThirdParty;
using Bluewire.Reporting.Cli.Jobs;
using Bluewire.Reporting.Cli.Model;
using Bluewire.Reporting.Cli.Scheduling;
using Bluewire.Reporting.Cli.ServiceProxy;
using Bluewire.Reporting.Cli.Sources;
using Bluewire.Reporting.Cli.Support;

namespace Bluewire.Reporting.Cli.Configuration
{
    public class ScheduleJobFactory : IJobFactory, IArgumentList
    {
        public IList<string> ArgumentList { get; } = new List<string>();
        public string SsrsUriString => ArgumentList.FirstOrDefault();
        public string SchedulingRule => ArgumentList.ElementAtOrDefault(1);

        public PathOnlyFilterDefinition ObjectFilter { get; } = new PathOnlyFilterDefinition();

        public ReportingServiceClientFactory ReportingServiceClientFactory { get; set; } = new ReportingServiceClientFactory();

        void IReceiveOptions.ReceiveFrom(OptionSet options)
        {
            options.AddCollector(ObjectFilter);
            options.Add("timeout=", "Number of seconds to wait for SSRS webservice responses", (int o) => ReportingServiceClientFactory.Timeout = TimeSpan.FromSeconds(o));
        }

        void IJobFactory.ConfigureSession(ConsoleSession<IJobFactory> session) => session.ListParameterUsage = "<ssrs-uri> <scheduling-rule>";

        public IJob CreateJob()
        {
            if (String.IsNullOrWhiteSpace(SsrsUriString)) throw new InvalidArgumentsException("No SSRS URI specified.");
            if (!Uri.TryCreate(SsrsUriString, UriKind.Absolute, out _)) throw new InvalidArgumentsException($"Not a valid absolute URI: {SsrsUriString}");
            if (String.IsNullOrWhiteSpace(SchedulingRule)) throw new InvalidArgumentsException("No scheduling rule specified.");

            var filter = new SsrsObjectFilter {
                ObjectTypes = SsrsFilterObjectTypes.Report,
                Path = new PathFilterExpression(
                    PathFilter.ParseGlob(ObjectFilter.IncludePaths) ?? PathFilter.MatchAll,
                    PathFilter.ParseGlob(ObjectFilter.ExcludePaths)
                )
            };
            var ssrsUri = new Uri(SsrsUriString, UriKind.Absolute);
            var service = ReportingServiceClientFactory.CreateFromShorthandUri(ssrsUri);

            var schedulingRule = new SchedulingRuleParser().Parse(SchedulingRule);
            var scheduler = new SsrsReportScheduler(service, schedulingRule);

            var source = new SsrsWebServiceObjectSource(service);
            var job = new ScheduleJob(source, scheduler, filter);
            return job;
        }
    }
}
