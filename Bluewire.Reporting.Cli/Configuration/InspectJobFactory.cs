using Bluewire.Common.Console.ThirdParty;
using Bluewire.Reporting.Cli.Jobs;
using Bluewire.Reporting.Cli.Support;
using System.Collections.Generic;
using System.Linq;
using Bluewire.Common.Console;
using Bluewire.Reporting.Cli.Model;
using Bluewire.Reporting.Cli.ServiceProxy;
using Bluewire.Reporting.Cli.Sources;

namespace Bluewire.Reporting.Cli.Configuration
{
    public class InspectJobFactory : IJobFactory, IArgumentList
    {
        public IList<string> ArgumentList { get; } = new List<string>();

        public ObjectFilterDefinition ObjectFilter { get; } = new ObjectFilterDefinition();
        public string Site { get; private set; }
        public SsrsObjectPath BasePath { get; private set; }

        public ReportingServiceClientFactory ReportingServiceClientFactory { get; set; } = new ReportingServiceClientFactory();

        void IReceiveOptions.ReceiveFrom(OptionSet options)
        {
            options.AddCollector(ObjectFilter);
            options.Add("site=", "Only include objects associated with a specific site", o => Site = o);
            options.Add("base=", "Specify a base path to assume for all items", o => BasePath = new SsrsObjectPath(o));
        }

        void IJobFactory.ConfigureSession(ConsoleSession<IJobFactory> session) => session.ListParameterUsage = "<ssrs-uri|file|directory>...";

        public IJob CreateJob()
        {
            var source = GetObjectSource();
            if (!source.HasManifest && Site != null)
            {
                throw new InvalidArgumentsException("--site was specified but no manifest was found.");
            }
            if (!new SsrsObjectTypesParser().ValidateTypes(ObjectFilter.TypeFilter, out var invalidTypes))
            {
                throw new InvalidArgumentsException($"Invalid object types: {invalidTypes}");
            }

            var filter = new SsrsObjectFilter {
                ObjectTypes = new SsrsObjectTypesParser().GetTypeFilter(ObjectFilter.TypeFilter),
                Path = new PathFilterExpression(
                    PathFilter.ParseGlob(ObjectFilter.IncludePaths) ?? PathFilter.MatchAll,
                    PathFilter.ParseGlob(ObjectFilter.ExcludePaths)
                ),
                Site = Site
            };
            return new InspectJob(source, filter);
        }

        private ISsrsObjectSource GetObjectSource()
        {
            if (!ArgumentList.Any()) throw new InvalidArgumentsException("No object source specified.");

            var sourceFactory = new ObjectSourceFactory() { ReportingServiceClientFactory = ReportingServiceClientFactory, BasePath = BasePath };
            return sourceFactory.Create(ArgumentList);
        }
    }
}
