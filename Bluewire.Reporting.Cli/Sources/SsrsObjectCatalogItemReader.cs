using System;
using System.Linq;
using System.Threading.Tasks;
using Bluewire.Reporting.Cli.Mapping;
using Bluewire.Reporting.Cli.ServiceProxy;
using Bluewire.Reporting.Common.Model;
using Bluewire.Reporting.Common.Sources.Ssrs;
using Task = System.Threading.Tasks.Task;

namespace Bluewire.Reporting.Cli.Sources
{
    public class SsrsObjectCatalogItemReader
    {
        private readonly IReportingServiceClient service;
        private TrustedUserHeader trustedUserHeader => new TrustedUserHeader();

        public SsrsObjectCatalogItemReader(IReportingServiceClient service)
        {
            this.service = service;
        }

        public async Task<SsrsObject> Read(CatalogItem item)
        {
            if (!Enum.TryParse<CatalogItemType>(item.TypeName, out var type)) return null;
            switch (type)
            {
                case CatalogItemType.DataSource: return await ReadDataSource(item);
                case CatalogItemType.DataSet: return await ReadDataSet(item);
                case CatalogItemType.Report: return await ReadReport(item);
            }
            return null;
        }

        public async Task<SsrsDataSource> ReadDataSource(CatalogItem item)
        {
            var response = await service.Proxy.GetDataSourceContentsAsync(new GetDataSourceContentsRequest {
                TrustedUserHeader = trustedUserHeader,
                DataSource = item.Path
            });
            return new SsrsDataSourceDefinitionMapper().MapToSsrsObject(item.Path, response.Definition);
        }

        public async Task<SsrsDataSet> ReadDataSet(CatalogItem item)
        {
            return new SsrsDataSet {
                Name = item.Name,
                Path = new SsrsObjectPath(item.Path),
                DataSourceReference = await GetDataSourceReference(item.Path),
                Definition = new ObjectDefinitionAccessor(service, item.Path)
            };
        }

        public Task<SsrsReport> ReadReport(CatalogItem item)
        {
            return Task.FromResult(new SsrsReport {
                Name = item.Name,
                Path = new SsrsObjectPath(item.Path),
                Definition = new ObjectDefinitionAccessor(service, item.Path)
            });
        }

        private async Task<SsrsObjectPath> GetDataSourceReference(string path)
        {
            var response = await service.Proxy.GetItemDataSourcesAsync(new GetItemDataSourcesRequest {
                TrustedUserHeader = trustedUserHeader,
                ItemPath = path
            });
            var source = response.DataSources.SingleOrDefault()?.Item;
            if (source is DataSourceReference reference) return new SsrsObjectPath(reference.Reference);
            return null;
        }

        class ObjectDefinitionAccessor : ISsrsObjectDefinition
        {
            private readonly IReportingServiceClient service;
            private readonly string path;

            public ObjectDefinitionAccessor(IReportingServiceClient service, string path)
            {
                this.service = service;
                this.path = path;
            }

            public async Task<byte[]> GetBytes()
            {
                var response = await service.Proxy.GetItemDefinitionAsync(new GetItemDefinitionRequest {
                    TrustedUserHeader = new TrustedUserHeader(),
                    ItemPath = path
                });
                return response.Definition;
            }
        }
    }
}
