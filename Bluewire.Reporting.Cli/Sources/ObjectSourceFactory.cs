using System;
using System.Collections.Generic;
using System.IO;
using Bluewire.Reporting.Cli.Model;
using Bluewire.Reporting.Cli.ServiceProxy;

namespace Bluewire.Reporting.Cli.Sources
{
    public class ObjectSourceFactory
    {
        public ReportingServiceClientFactory ReportingServiceClientFactory { get; set; } = new ReportingServiceClientFactory();
        public SsrsObjectPath BasePath { get; set; }

        public ISsrsObjectSource Create(IEnumerable<string> sourceIdentifiers)
        {
            var aggregateSource = new AggregatedObjectSource();
            try
            {
                foreach (var sourceIdentifier in sourceIdentifiers)
                {
                    aggregateSource.Add(Create(sourceIdentifier));
                }
                return aggregateSource;
            }
            catch
            {
                aggregateSource.Dispose();
                throw;
            }
        }

        public ISsrsObjectSource Create(string sourceIdentifier)
        {
            if (Uri.TryCreate(sourceIdentifier, UriKind.Absolute, out var uri))
            {
                if (uri.IsFile) return CreateFromFilesystemPath(uri.LocalPath);
                var service = ReportingServiceClientFactory.CreateFromShorthandUri(uri);
                return new SsrsWebServiceObjectSource(service);
            }
            return CreateFromFilesystemPath(sourceIdentifier);
        }

        public ISsrsObjectSource CreateFromFilesystemPath(string filesystemPath)
        {
            var fullPath = Path.GetFullPath(filesystemPath);
            if (Directory.Exists(fullPath))
            {
                return new DirectoryObjectSource(fullPath, BasePath);
            }
            if (File.Exists(fullPath))
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(Path.GetExtension(fullPath), ".zip"))
                {
                    return new ZipFileObjectSource(fullPath, BasePath);
                }
                return new SingleFileObjectSource(fullPath, BasePath);
            }
            throw new FileNotFoundException("Path does not refer to an existing file or directory.", fullPath);
        }
    }
}
