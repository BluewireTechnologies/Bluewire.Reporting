using System.Xml.Linq;

namespace Bluewire.Reporting.Cli.Xml
{
    public class DataSetXmlSchema
    {
        public static readonly XNamespace Xmlns = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/shareddatasetdefinition";

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
            return xml
                .Element(Xmlns + "SharedDataSet")
                .Element(Xmlns + "DataSet")
                .Element(Xmlns + "Query")
                .Element(Xmlns + "DataSourceReference");
        }
    }
}
