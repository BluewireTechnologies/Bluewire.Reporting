using System;
using System.Collections.Generic;
using System.Linq;
using Bluewire.Common.Console;
using Bluewire.Common.Console.ThirdParty;
using Bluewire.Reporting.Cli.Jobs;
using Bluewire.Reporting.Cli.Model;
using Bluewire.Reporting.Cli.Rewrite;
using Bluewire.Reporting.Cli.ServiceProxy;
using Bluewire.Reporting.Cli.Sources;
using Bluewire.Reporting.Cli.Support;

namespace Bluewire.Reporting.Cli.Configuration
{
    public class ImportJobFactory : IJobFactory, IArgumentList
    {
        public IList<string> ArgumentList { get; } = new List<string>();
        public string SsrsUriString => ArgumentList.FirstOrDefault();
        public IEnumerable<string> SourcePaths => ArgumentList.Skip(1);

        public ObjectFilterDefinition ObjectFilter { get; } = new ObjectFilterDefinition();
        public string Site { get; private set; }
        public SsrsObjectPath BasePath { get; private set; }
        public bool Overwrite { get; set; }
        public List<string> RewriteRules { get; } = new List<string>();

        public ReportingServiceClientFactory ReportingServiceClientFactory { get; set; } = new ReportingServiceClientFactory();

        void IReceiveOptions.ReceiveFrom(OptionSet options)
        {
            options.AddCollector(ObjectFilter);
            options.Add("site=", "Only include objects associated with a specific site", o => Site = o.Unquote("'"));
            options.Add("base=", "Specify a base path to assume for all items", o => BasePath = new SsrsObjectPath(o));
            options.Add("overwrite", "Replace existing objects", o => Overwrite = true);
            options.Add("rewrite=", "Modify objects prior to import", o => RewriteRules.Add(o.Unquote("'")));
        }

        void IJobFactory.ConfigureSession(ConsoleSession<IJobFactory> session) => session.ListParameterUsage = "<ssrs-uri> <files|directories...>";

        public IJob CreateJob()
        {
            if (String.IsNullOrWhiteSpace(SsrsUriString)) throw new InvalidArgumentsException("No SSRS URI specified.");
            if (!Uri.TryCreate(SsrsUriString, UriKind.Absolute, out _)) throw new InvalidArgumentsException($"Not a valid absolute URI: {SsrsUriString}");
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
            var ssrsUri = new Uri(SsrsUriString, UriKind.Absolute);
            var service = ReportingServiceClientFactory.CreateFromShorthandUri(ssrsUri);
            var job = new ImportJob(service, source, filter) { Overwrite = Overwrite };
            foreach (var rule in RewriteRules)
            {
                var rewriter = new RewriteRuleParser().Parse(rule);
                job.Rewriters.Add(rewriter);
            }
            return job;
        }

        private ISsrsObjectSource GetObjectSource()
        {
            if (!SourcePaths.Any()) throw new InvalidArgumentsException("No source paths specified.");

            var sourceFactory = new ObjectSourceFactory() { ReportingServiceClientFactory = ReportingServiceClientFactory, BasePath = BasePath };
            var aggregateSource = new AggregatedObjectSource();
            try
            {
                foreach (var path in SourcePaths)
                {
                    aggregateSource.Add(sourceFactory.Create(path));
                }
                return aggregateSource;
            }
            catch
            {
                aggregateSource.Dispose();
                throw;
            }
        }
    }
}
