using System;
using System.Threading.Tasks;
using Bluewire.Reporting.Common.Model;
using Bluewire.Reporting.Common.Sources.Ssrs;
using log4net;

namespace Bluewire.Reporting.Cli.ServiceProxy
{
    public static class ReportingServiceClientExtensions
    {
        public static async Task<string> GetOrCreateContainer(this IReportingServiceClient service, SsrsObjectPath path, ILog log = null)
        {
            var container = path.Parent;
            if (container.IsRoot) return container.ToString();
            if (await ItemExists(service, container)) return container.ToString();

            var parentContainer = await GetOrCreateContainer(service, container, log);
            log?.DebugFormat("Creating folder: '{0}'", container);
            var response = await service.Proxy.CreateFolderAsync(new CreateFolderRequest {
                TrustedUserHeader = new TrustedUserHeader(),
                Folder = container.Name,
                Parent = parentContainer
            });
            return response.ItemInfo.Path;
        }

        public static async Task<bool> ItemExists(this IReportingServiceClient service, SsrsObjectPath path, ILog log = null)
        {
            var response = await service.Proxy.GetItemTypeAsync(new GetItemTypeRequest {
                TrustedUserHeader = new TrustedUserHeader(),
                ItemPath = path.ToString()
            });
            if (!Enum.TryParse<CatalogItemType>(response.Type, out var type) || type == CatalogItemType.Unknown)
            {
                log?.DebugFormat("Item does not exist: '{0}', type = '{1}'", path, response.Type);
                return false;
            }
            log?.DebugFormat("Item exists: '{0}', type = '{1}'", path, response.Type);
            return true;
        }

        public static void LogServiceWarnings(this ILog log, Warning[] warnings)
        {
            if (warnings == null) return;
            foreach (var warning in warnings)
            {
                log.Warn(warning.Message);
            }
        }
    }
}
