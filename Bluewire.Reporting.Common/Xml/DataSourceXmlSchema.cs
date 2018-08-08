using System.Xml.Linq;

namespace Bluewire.Reporting.Cli.Xml
{
    public class DataSourceXmlSchema
    {
        public string GetConnectionString(XDocument xml)
        {
            return xml
                .Element("RptDataSource")
                .Element("ConnectionProperties")
                .Element("ConnectString")
                .Value;
        }

        public bool GetIntegratedSecurity(XDocument xml)
        {
            return xml
                .Element("RptDataSource")
                .Element("ConnectionProperties")
                .Element("IntegratedSecurity")
                .GetBoolean();
        }
    }
}
