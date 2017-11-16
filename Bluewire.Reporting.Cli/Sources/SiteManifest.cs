using System.Collections.Generic;
using System.Linq;

namespace Bluewire.Reporting.Cli.Sources
{
    public class SiteManifest
    {
        private readonly ILookup<string, IPathFilter> rules;

        public SiteManifest(ILookup<string, IPathFilter> rules)
        {
            this.rules = rules;
        }

        public IPathFilter GetSiteFilter(string siteName)
        {
            var siteRules = rules[siteName].ToArray();
            if (!siteRules.Any()) return PathFilter.MatchNone;
            return new SiteManifestFilter(siteName, siteRules);
        }

        class SiteManifestFilter : IPathFilter
        {
            private readonly string siteName;
            private readonly IPathFilter[] filters;

            public SiteManifestFilter(string siteName, IPathFilter[] filters)
            {
                this.siteName = siteName;
                this.filters = filters;
            }

            public bool Matches(string path) => filters.Any(f => f.Matches(path));
            public IEnumerable<string> PrefixSegments => Enumerable.Empty<string>();
            public override string ToString() => $"<Site:{siteName}>";
        }
    }
}
