using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Bluewire.Reporting.Common.Scheduling
{
    public class RdlCustomPropertiesReader
    {
        public IDictionary<string, string> GetPropertiesFromRdl(XDocument rdl)
        {
            var report = rdl.Element("Report");
            if (report == null) return new Dictionary<string, string>();;

            return report.Elements("CustomProperties").Elements("CustomProperty")
                .ToDictionary(p => p.Element("Name")?.Value, p => p.Element("Value")?.Value);
        }
    }
}
