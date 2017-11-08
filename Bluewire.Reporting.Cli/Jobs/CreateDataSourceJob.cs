using System;
using System.IO;
using Bluewire.Reporting.Cli.Mapping;
using Bluewire.Reporting.Cli.Model;
using Bluewire.Reporting.Cli.ServiceProxy;
using log4net;
using Task = System.Threading.Tasks.Task;

namespace Bluewire.Reporting.Cli.Jobs
{
    public class CreateDataSourceJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CreateDataSourceJob));
        private readonly IReportingServiceClient service;
        private readonly SsrsDataSource dataSource;
        private TrustedUserHeader trustedUserHeader => new TrustedUserHeader();

        public bool Overwrite { get; set; }

        public CreateDataSourceJob(
            IReportingServiceClient service,
            SsrsDataSource dataSource
            )
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        }

        public async Task Run(TextWriter output)
        {
            var definition = new DataSourceDefinition();
            new SsrsDataSourceDefinitionMapper().MapFromSsrsObject(definition, dataSource);

            var parentPath = await service.GetOrCreateContainer(dataSource.Path, log);
            log.InfoFormat("Creating data source: '{0}'", dataSource.Path);
            await service.Proxy.CreateDataSourceAsync(new CreateDataSourceRequest {
                TrustedUserHeader = trustedUserHeader,
                DataSource = dataSource.Name,
                Parent = parentPath,
                Overwrite = Overwrite,
                Definition = definition
            });
        }
    }
}
