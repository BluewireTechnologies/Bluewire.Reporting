using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bluewire.Reporting.Cli.Model;
using log4net;

namespace Bluewire.Reporting.Cli.Sources
{
    public class DirectoryObjectSource : ISsrsObjectSource
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DirectoryObjectSource));
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
            var siteFilter = GetSiteFilter(filter.Site);
            foreach (var relativePath in new RelativeDirectoryExplorer().EnumerateRelativeFiles(rootContainer))
            {
                var containerPath = basePath + SsrsObjectPath.FromFileSystemPath(Path.GetDirectoryName(relativePath));
                var ssrsObject = new SsrsObjectFileReader().Read(Path.Combine(directoryPath, relativePath), containerPath);
                if (filter.Excludes(ssrsObject)) continue;
                if (siteFilter?.Matches(ssrsObject.Path) == false) continue;
                yield return ssrsObject;
            }
        }

        private IPathFilter GetSiteFilter(string siteName)
        {
            if (String.IsNullOrWhiteSpace(siteName)) return null;
            var manifestFilePath = Path.Combine(directoryPath, ManifestFilePath);
            if (!File.Exists(manifestFilePath)) return null;

            log.InfoFormat("Found manifest file '{0}'", manifestFilePath);

            using (var stream = new FileStream(manifestFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var manifest = new SiteManifestReader(basePath).Read(stream);
                return manifest.GetSiteFilter(siteName);
            }
        }

        public void Dispose()
        {
        }
    }
}
