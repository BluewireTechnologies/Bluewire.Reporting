using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Bluewire.Reporting.Cli.Model;

namespace Bluewire.Reporting.Cli.Sources
{
    public class SiteManifestReader
    {
        private readonly SsrsObjectPath basePath;

        public SiteManifestReader(SsrsObjectPath basePath)
        {
            this.basePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
        }

        public SiteManifest Read(Stream manifestStream)
        {
            var xml = XDocument.Load(manifestStream);
            var ruleDefinitions = xml.Elements("Reports")
                .Elements("Report")
                .SelectMany(e => e.Elements("Include")
                    .Select(i => new {
                        SiteName = i.Attribute("Site")?.Value,
                        PathPrefix = e.Attribute("Path")?.Value
                    }))
                .Where(e => !String.IsNullOrWhiteSpace(e.SiteName) && e.PathPrefix != null)
                .Distinct();

            var rules = ruleDefinitions.ToLookup(
                r => r.SiteName,
                r => PathFilter.FromPrefix(basePath + r.PathPrefix) ?? PathFilter.MatchAll,
                StringComparer.OrdinalIgnoreCase);

            return new SiteManifest(rules);
        }
    }
}
