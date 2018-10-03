using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bluewire.Reporting.Cli.ServiceProxy;
using Bluewire.Reporting.Common.Model;
using Bluewire.Reporting.Common.Sources;
using Bluewire.Reporting.Common.Sources.Ssrs;

namespace Bluewire.Reporting.Cli.Sources
{
    public class SsrsWebServiceObjectSource : ISsrsObjectSource
    {
        private readonly IReportingServiceClient service;
        private TrustedUserHeader trustedUserHeader => new TrustedUserHeader();

        public SsrsWebServiceObjectSource(IReportingServiceClient service)
        {
            this.service = service;
        }

        public async Task<IEnumerable<SsrsObject>> Enumerate(SsrsObjectFilter filter)
        {
            var container = "/" + String.Join("/", filter.Path.PrefixSegments);
            var response = await service.Proxy.ListChildrenAsync(new ListChildrenRequest {
                TrustedUserHeader = trustedUserHeader,
                ItemPath = container,
                Recursive = true
            });
            return await MapCatalogItemsToSsrsObjects(response.CatalogItems, filter);
        }

        public bool HasManifest => false;

        private async Task<SsrsObject[]> MapCatalogItemsToSsrsObjects(IEnumerable<CatalogItem> items, SsrsObjectFilter filter)
        {
            var objects = new List<SsrsObject>();
            foreach (var item in items)
            {
                if (!filter.Path.Matches(item.Path)) continue;
                if (!Enum.TryParse<CatalogItemType>(item.TypeName, out var type)) continue;
                if (!MatchesTypeFilter(filter.ObjectTypes, type)) continue;
                var ssrsObject = await new SsrsObjectCatalogItemReader(service).Read(item);
                if (ssrsObject == null) continue;
                objects.Add(ssrsObject);
            }
            return objects.ToArray();
        }

        private static bool MatchesTypeFilter(SsrsFilterObjectTypes filterTypes, CatalogItemType type)
        {
            if (filterTypes == SsrsFilterObjectTypes.All) return true;
            switch (type)
            {
                case CatalogItemType.DataSource: return (filterTypes & SsrsFilterObjectTypes.DataSource) != 0;
                case CatalogItemType.Report:     return (filterTypes & SsrsFilterObjectTypes.Report) != 0;
                case CatalogItemType.DataSet:    return (filterTypes & SsrsFilterObjectTypes.DataSet) != 0;
            }
            return false;
        }

        public void Dispose()
        {
            service.Dispose();
        }
    }
}
