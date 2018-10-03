using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bluewire.Common.Console;
using Bluewire.Common.Console.Arguments;
using Bluewire.Common.Console.ThirdParty;
using Bluewire.Reporting.Cli.Exports;
using Bluewire.Reporting.Cli.Jobs;
using Bluewire.Reporting.Cli.ServiceProxy;
using Bluewire.Reporting.Cli.Sources;
using Bluewire.Reporting.Cli.Support;
using Bluewire.Reporting.Common.Model;
using Bluewire.Reporting.Common.Rewrite;
using Bluewire.Reporting.Common.Sources;

namespace Bluewire.Reporting.Cli.Configuration
{
    public class ImportJobFactory : IJobFactory, IReceiveArgumentList
    {
        public string SsrsUriString { get; set; }
        public IList<string> SourcePaths { get; } = new List<string>();

        public ObjectFilterDefinition ObjectFilter { get; } = new ObjectFilterDefinition();
        public string Site { get; private set; }
        public SsrsObjectPath BasePath { get; private set; }
        public bool Overwrite { get; set; }
        public string BackupPath { get; set; }
        public List<string> RewriteRules { get; } = new List<string>();

        public ReportingServiceClientFactory ReportingServiceClientFactory { get; set; } = new ReportingServiceClientFactory();

        void IReceiveOptions.ReceiveFrom(OptionSet options)
        {
            options.AddCollector(ObjectFilter);
            options.Add("site=", "Only include objects associated with a specific site", o => Site = o.Unquote("'"));
            options.Add("base=", "Specify a base path to assume for all items", o => BasePath = new SsrsObjectPath(o));
            options.Add("overwrite", "Replace existing objects", o => Overwrite = true);
            options.Add("backup=", "Back up objects before overwriting", o => BackupPath = o);
            options.Add("rewrite=", "Modify objects prior to import", o => RewriteRules.Add(o.Unquote("'")));
            options.Add("timeout=", "Number of seconds to wait for SSRS webservice responses", (int o) => ReportingServiceClientFactory.Timeout = TimeSpan.FromSeconds(o));
        }

        void IReceiveArgumentList.ReceiveFrom(ArgumentList argumentList)
        {
            argumentList.Add("ssrs-uri", o => SsrsUriString = o);
            argumentList.AddRemainder("files|directories...", o => SourcePaths.Add(o));
        }

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
            var job = new ImportJob(service, source, filter) { Overwrite = Overwrite, BackupTarget = GetBackupTarget() };
            foreach (var rule in RewriteRules)
            {
                var rewriter = new RewriteRuleParser().Parse(rule);
                job.Rewriters.Add(rewriter);
            }
            return job;
        }

        private IObjectExportTarget GetBackupTarget()
        {
            if (String.IsNullOrWhiteSpace(BackupPath)) return null;
            if (!Overwrite) throw new InvalidArgumentsException("--backup cannot be used without --overwrite.");
            return new ExportToDirectoryHierarchy(Path.GetFullPath(BackupPath));
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
