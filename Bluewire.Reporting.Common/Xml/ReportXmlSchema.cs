using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Bluewire.Reporting.Common.Xml
{
    public class ReportXmlSchema
    {
        public static readonly XNamespace Xmlns2010 = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition";

        private static readonly Regex rxReportNamespace = new Regex(@"^http://schemas.microsoft.com/sqlserver/reporting/(?<year>\d+)/(?<month>\d+)/reportdefinition$");
        public static bool IsReportNamespace(XNamespace xmlns, out int year, out int month)
        {
            var m = rxReportNamespace.Match(xmlns.NamespaceName);
            if (m.Success)
            {
                year = int.Parse(m.Groups["year"].Value);
                month = int.Parse(m.Groups["month"].Value);
                return true;
            }
            year = default(int);
            month = default(int);
            return false;
        }
    }
}
