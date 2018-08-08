using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bluewire.Reporting.Common.Model;

namespace Bluewire.Reporting.Common.Sources
{
    public class SingleFileObjectSource : ISsrsObjectSource
    {
        private readonly string filePath;
        private readonly SsrsObjectPath basePath;

        public SingleFileObjectSource(string filePath, SsrsObjectPath basePath = null)
        {
            if (String.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
            if (!Path.IsPathRooted(filePath)) throw new ArgumentException($"Not an absolute path: {filePath}");

            this.filePath = filePath;
            this.basePath = basePath;
        }

        public Task<IEnumerable<SsrsObject>> Enumerate(SsrsObjectFilter filter)
        {
            return Task.FromResult(ReadWithFilter(filter));
        }

        private IEnumerable<SsrsObject> ReadWithFilter(SsrsObjectFilter filter)
        {
            var ssrsObject = new SsrsObjectFileReader().Read(filePath, basePath);
            if (filter.Excludes(ssrsObject)) yield break;
            yield return ssrsObject;
        }

        public bool HasManifest => false;

        public void Dispose()
        {
        }
    }
}
