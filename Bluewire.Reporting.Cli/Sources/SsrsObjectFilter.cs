using Bluewire.Reporting.Cli.Model;

namespace Bluewire.Reporting.Cli.Sources
{
    public class SsrsObjectFilter
    {
        public SsrsFilterObjectTypes ObjectTypes { get; set; }
        public IPathFilter Path { get; set; }
        public string Site { get; set; }
    }

    public static class SsrsObjectFilterExtensions
    {
        /// <summary>
        /// Returns true if the specified object is rejected by path or by type.
        /// </summary>
        public static bool Excludes(this SsrsObjectFilter filter, SsrsObject obj)
        {
            if (obj == null) return true;
            if (!filter.Path.Matches(obj.Path)) return true;

            if (filter.ObjectTypes != SsrsFilterObjectTypes.All)
            {
                switch (obj.Type)
                {
                    case SsrsObjectType.DataSource: return (filter.ObjectTypes & SsrsFilterObjectTypes.DataSource) == 0;
                    case SsrsObjectType.Report: return (filter.ObjectTypes & SsrsFilterObjectTypes.Report) == 0;
                    case SsrsObjectType.DataSet: return (filter.ObjectTypes & SsrsFilterObjectTypes.DataSet) == 0;
                }
                return true;
            }
            return false;
        }
    }
}
