using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Bluewire.Reporting.Cli.Xml
{
    public class DataSetXmlSchema
    {
        public static readonly XNamespace Xmlns2010 = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/shareddatasetdefinition";

        private static readonly Regex rxDataSetNamespace = new Regex(@"^http://schemas.microsoft.com/sqlserver/reporting/(?<year>\d+)/(?<month>\d+)/shareddatasetdefinition");
        public static bool IsDataSetNamespace(XNamespace xmlns, out int year, out int month)
        {
            var m = rxDataSetNamespace.Match(xmlns.NamespaceName);
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

        public string GetDataSourceReference(XDocument xml)
        {
            return DataSourceReferenceElement(xml).Value;
        }

        public void SetDataSourceReference(XDocument xml, string reference)
        {
            DataSourceReferenceElement(xml).Value = reference;
        }

        private XElement DataSourceReferenceElement(XDocument xml)
        {
            if (xml == null) throw new ArgumentNullException(nameof(xml));
            var xmlns = xml.Root?.Name.Namespace;
            if (xmlns == null) throw new ArgumentException("Unable to determine root element namespace.");
            return xml
                .Element(xmlns + "SharedDataSet")?
                .Element(xmlns + "DataSet")?
                .Element(xmlns + "Query")?
                .Element(xmlns + "DataSourceReference");
        }
    }
}
