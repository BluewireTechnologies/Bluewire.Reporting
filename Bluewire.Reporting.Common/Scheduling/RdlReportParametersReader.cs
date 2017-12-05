using System.Linq;
using System.Xml.Linq;

namespace Bluewire.Reporting.Common.Scheduling
{
    public class RdlReportParametersReader
    {
        public string[] GetParameterNamesFromRdl(XDocument rdl)
        {
            var report = rdl.Element("Report");
            if (report == null) return new string[0];

            return report.Elements("ReportParameters").Elements("ReportParameter")
                .Select(p => p.Attribute("Name")?.Value)
                .ToArray();
        }
    }
}
