using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Bluewire.Reporting.Cli.Mapping;
using Bluewire.Reporting.Cli.Model;
using Bluewire.Reporting.Cli.ServiceProxy;
using Bluewire.Reporting.Cli.Sources;
using Bluewire.Reporting.Cli.Sources.Ssrs;
using log4net;
using Task = System.Threading.Tasks.Task;

namespace Bluewire.Reporting.Cli.Jobs
{
    public class ImportJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ImportJob));
        private readonly IReportingServiceClient service;
        private readonly ISsrsObjectSource source;
        private readonly SsrsObjectFilter filter;
        private TrustedUserHeader trustedUserHeader => new TrustedUserHeader();

        public bool Overwrite { get; set; }

        public ImportJob(
            IReportingServiceClient service,
            ISsrsObjectSource source,
            SsrsObjectFilter filter
            )
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public async Task Run(TextWriter output)
        {
            log.Info("Collecting objects...");
            var items = (await source.Enumerate(filter)).ToArray();

            log.Info("Importing data sources...");
            foreach (var item in items.OfType<SsrsDataSource>())
            {
                await ImportDataSource(item);
            }
            log.Info("Importing datasets...");
            foreach (var item in items.OfType<SsrsDataSet>())
            {
                await ImportDataSet(item);
            }
            log.Info("Importing reports...");
            foreach (var item in items.OfType<SsrsReport>())
            {
                await ImportReport(item);
            }
            log.Info("Done.");
        }

        private async Task ImportDataSource(SsrsDataSource item)
        {
            var definition = new DataSourceDefinition();
            new SsrsDataSourceDefinitionMapper().MapFromSsrsObject(definition, item);

            var parentPath = await service.GetOrCreateContainer(item.Path, log);
            log.DebugFormat("Importing data source: '{0}'", item.Path);
            try
            {
                await service.Proxy.CreateDataSourceAsync(new CreateDataSourceRequest {
                    TrustedUserHeader = trustedUserHeader,
                    DataSource = item.Name,
                    Parent = parentPath,
                    Overwrite = Overwrite,
                    Definition = definition
                });
            }
            catch (FaultException)
            {
                if (Overwrite) throw;   // Failure wasn't caused by existence of item.
                if (!await service.ItemExists(item.Path)) throw;
            }
        }

        private async Task ImportDataSet(SsrsDataSet item)
        {
            var parentPath = await service.GetOrCreateContainer(item.Path, log);
            log.DebugFormat("Importing dataset: '{0}'", item.Path);
            try
            {
                var response = await service.Proxy.CreateCatalogItemAsync(new CreateCatalogItemRequest {
                    TrustedUserHeader = trustedUserHeader,
                    ItemType = CatalogItemType.DataSet.ToString(),
                    Name = item.Name,
                    Parent = parentPath,
                    Overwrite = Overwrite,
                    Definition = await item.Definition.GetBytes()
                });
                log.LogServiceWarnings(response.Warnings);
            }
            catch (FaultException)
            {
                if (Overwrite) throw;   // Failure wasn't caused by existence of item.
                if (!await service.ItemExists(item.Path)) throw;
            }
            // The Overwrite option doesn't replace the definition for some reason.
            await service.Proxy.SetItemDataSourcesAsync(new SetItemDataSourcesRequest {
                TrustedUserHeader = trustedUserHeader,
                ItemPath = item.Path,
                DataSources = new[] {
                    new DataSource {
                        Name = "DataSetDataSource",
                        Item = new DataSourceReference { Reference = item.DataSourceReference }
                    }
                }
            });
        }

        private async Task ImportReport(SsrsReport item)
        {
            var parentPath = await service.GetOrCreateContainer(item.Path, log);
            log.DebugFormat("Importing report: '{0}'", item.Path);
            try
            {
                var response = await service.Proxy.CreateCatalogItemAsync(new CreateCatalogItemRequest {
                    TrustedUserHeader = trustedUserHeader,
                    ItemType = CatalogItemType.Report.ToString(),
                    Name = item.Name,
                    Parent = parentPath,
                    Overwrite = Overwrite,
                    Definition = await item.Definition.GetBytes()
                });
                log.LogServiceWarnings(response.Warnings);
            }
            catch (FaultException)
            {
                if (Overwrite) throw;   // Failure wasn't caused by existence of item.
                if (!await service.ItemExists(item.Path)) throw;
                return;
            }
            await service.Proxy.SetExecutionOptionsAsync(new SetExecutionOptionsRequest {
                TrustedUserHeader = trustedUserHeader,
                ExecutionSetting = "Snapshot",
                ItemPath = item.Path.ToString(),
                Item = new NoSchedule()
            });
            await service.Proxy.SetItemHistoryOptionsAsync(new SetItemHistoryOptionsRequest {
                TrustedUserHeader = trustedUserHeader,
                EnableManualSnapshotCreation = true,
                KeepExecutionSnapshots = false,
                ItemPath = item.Path.ToString(),
                Item = new NoSchedule()
            });

            await FixupEproParameters(item);
        }

        private async Task FixupEproParameters(SsrsReport item)
        {
            var parametersResponse = await service.Proxy.GetItemParametersAsync(new GetItemParametersRequest {
                TrustedUserHeader = trustedUserHeader,
                ItemPath = item.Path.ToString(),
                ForRendering = false
            });

            var parameters = parametersResponse.Parameters.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            if (parameters.TryGetValue("year", out var yearParam)) yearParam.PromptUser = false;
            if (parameters.TryGetValue("month", out var monthParam)) monthParam.PromptUser = false;
            if (parameters.TryGetValue("day", out var dayParam)) dayParam.PromptUser = false;

            await service.Proxy.SetItemParametersAsync(new SetItemParametersRequest {
                TrustedUserHeader = trustedUserHeader,
                ItemPath = item.Path.ToString(),
                Parameters = parametersResponse.Parameters
            });
        }
    }
}
