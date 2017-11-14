using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Bluewire.Reporting.Cli.Model;

namespace Bluewire.Reporting.Cli.Sources
{
    public class ZipFileObjectSource : ISsrsObjectSource
    {
        private readonly string zipFilePath;
        private readonly SsrsObjectPath basePath;
        private ZipArchive zip;

        private const string ManifestFilePath = "report-manifest.xml";

        public ZipFileObjectSource(string zipFilePath, SsrsObjectPath basePath = null)
        {
            this.zipFilePath = zipFilePath ?? throw new ArgumentNullException(nameof(zipFilePath));
            this.basePath = basePath;
        }

        public Task<IEnumerable<SsrsObject>> Enumerate(SsrsObjectFilter filter)
        {
            EnsureOpened();
            return Task.FromResult(MapEntriesToSsrsObjects(zip.Entries.ToArray(), filter));
        }

        public bool HasManifest => GetManifestEntryName() != null;

        private IEnumerable<SsrsObject> MapEntriesToSsrsObjects(ZipArchiveEntry[] items, SsrsObjectFilter filter)
        {
            foreach (var item in items)
            {
                var containerPath = basePath + SsrsObjectPath.FromFileSystemPath(Path.GetDirectoryName(item.FullName));
                using (var stream = item.Open())
                {
                    var ssrsObject = new SsrsObjectFileReader().Read(stream, Path.GetFileNameWithoutExtension(item.Name), containerPath);
                    if (filter.Excludes(ssrsObject)) continue;
                    yield return ssrsObject;
                }
            }
        }

        private void EnsureOpened()
        {
            if (zip != null) return;
            var zipStream = File.Open(zipFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
            }
            catch
            {
                zipStream.Dispose();
                throw;
            }
        }

        private string GetManifestEntryName()
        {
            EnsureOpened();
            return zip.Entries
                .Select(e => e.FullName)
                .Where(f => StringComparer.OrdinalIgnoreCase.Equals(f, ManifestFilePath))
                .SingleOrDefault();
        }

        public void Dispose()
        {
            zip?.Dispose();
            zip = null;
        }
    }
}
