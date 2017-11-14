using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bluewire.Reporting.Cli.Model;

namespace Bluewire.Reporting.Cli.Sources
{
    public class DirectoryObjectSource : ISsrsObjectSource
    {
        private readonly string directoryPath;
        private readonly SsrsObjectPath basePath;

        private const string ManifestFilePath = "report-manifest.xml";

        public DirectoryObjectSource(string directoryPath, SsrsObjectPath basePath = null)
        {
            this.directoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            this.basePath = basePath;
        }

        public Task<IEnumerable<SsrsObject>> Enumerate(SsrsObjectFilter filter)
        {
            return Task.FromResult(MapFilesToSsrsObjects(filter));
        }

        public bool HasManifest => File.Exists(Path.Combine(directoryPath, ManifestFilePath));

        private IEnumerable<SsrsObject> MapFilesToSsrsObjects(SsrsObjectFilter filter)
        {
            var rootContainer = new DirectoryInfo(directoryPath);
            if (!rootContainer.Exists) yield break;
            foreach (var relativePath in new RelativeDirectoryExplorer().EnumerateRelativeFiles(rootContainer))
            {
                var containerPath = basePath + SsrsObjectPath.FromFileSystemPath(Path.GetDirectoryName(relativePath));
                var ssrsObject = new SsrsObjectFileReader().Read(Path.Combine(directoryPath, relativePath), containerPath);
                if (filter.Excludes(ssrsObject)) continue;
                yield return ssrsObject;
            }
        }

        public void Dispose()
        {
        }
    }
}
