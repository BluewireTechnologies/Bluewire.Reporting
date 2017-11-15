using System;
using System.IO;
using System.Xml.Linq;
using Bluewire.Reporting.Cli.Model;
using Task = System.Threading.Tasks.Task;

namespace Bluewire.Reporting.Cli.Exports
{
    public class ExportToDirectoryHierarchy : IObjectExportTarget
    {
        private readonly string basePath;

        public ExportToDirectoryHierarchy(string basePath)
        {
            this.basePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
            if (!Path.IsPathRooted(basePath)) throw new ArgumentException("Path must be absolute", nameof(basePath));
        }

        private string GetOrCreateContainer(SsrsObjectPath path)
        {
            var container = path.Parent;
            var directoryPath = Path.Combine(basePath, container.AsRelativeFileSystemPath());
            Directory.CreateDirectory(directoryPath);
            return directoryPath;
        }

        public async Task WriteObject(SsrsObject item)
        {
            var directoryPath = GetOrCreateContainer(item.Path);
            if (item is SsrsDataSource dataSource) await ExportDataSource(directoryPath, dataSource);
            if (item is SsrsDataSet dataSet) await ExportDataSet(directoryPath, dataSet);
            if (item is SsrsReport report) await ExportReport(directoryPath, report);
        }

        private Task ExportDataSource(string directoryPath, SsrsDataSource dataSource)
        {
            var filePath = Path.Combine(directoryPath, $"{dataSource.Name}.rds");
            var xml = new XDocument(
                new XElement("RptDataSource",
                    new XAttribute("Name", dataSource.Name),
                    new XElement("ConnectionProperties",
                        new XElement("Extension", "SQL"),
                        new XElement("ConnectString", dataSource.ConnectionString),
                        new XElement("IntegratedSecurity", dataSource.Authentication as SsrsDataSource.AuthenticationType.WindowsIntegrated)),
                    new XElement("DataSourceID")));
            xml.Save(filePath);
            return Task.CompletedTask;
        }

        private async Task ExportDataSet(string directoryPath, SsrsDataSet dataSet)
        {
            var filePath = Path.Combine(directoryPath, $"{dataSet.Name}.rsd");
            using (var stream = File.OpenWrite(filePath))
            {
                var bytes = await dataSet.Definition.GetBytes();
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        private async Task ExportReport(string directoryPath, SsrsReport report)
        {
            var filePath = Path.Combine(directoryPath, $"{report.Name}.rdl");
            using (var stream = File.OpenWrite(filePath))
            {
                var bytes = await report.Definition.GetBytes();
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}
