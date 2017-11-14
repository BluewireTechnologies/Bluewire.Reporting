using System;
using System.Collections.Generic;

namespace Bluewire.Reporting.Cli.Sources
{
    public class PathFilterExpression : IPathFilter
    {
        private readonly IPathFilter include;
        private readonly IPathFilter exclude;

        public PathFilterExpression(IPathFilter include, IPathFilter exclude = null)
        {
            this.include = include ?? throw new ArgumentNullException(nameof(include));
            this.exclude = exclude ?? PathFilter.MatchNone;
        }

        public bool Matches(string path)
        {
            return include.Matches(path) && !exclude.Matches(path);
        }

        public IEnumerable<string> PrefixSegments => include.PrefixSegments;
    }
}
