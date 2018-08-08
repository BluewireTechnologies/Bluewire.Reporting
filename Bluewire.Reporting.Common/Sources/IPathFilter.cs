using System.Collections.Generic;

namespace Bluewire.Reporting.Common.Sources
{
    public interface IPathFilter
    {
        bool Matches(string path);
        IEnumerable<string> PrefixSegments { get; }
    }
}
