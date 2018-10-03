using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Bluewire.Reporting.Common.Model;
using Bluewire.Reporting.Common.Support;
using Bluewire.Reporting.Common.Xml;

namespace Bluewire.Reporting.Common.Sources
{
    public class SsrsObjectFileReader
    {
        public SsrsObject Read(string filePath, SsrsObjectPath containerPath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (!TryLoadXml(fileStream, out var xml)) return null;
                return ReadXml(xml, Path.GetFileNameWithoutExtension(filePath), containerPath);
            }
        }

        public SsrsObject Read(Stream stream, string itemName, SsrsObjectPath containerPath)
        {
            if (!TryLoadXml(stream, out var xml)) return null;
            return ReadXml(xml, itemName, containerPath);
        }

        private bool TryLoadXml(Stream stream, out XDocument xml)
        {
            try
            {
                xml = XDocument.Load(stream);
                return true;
            }
            catch (XmlException)
            {
                xml = null;
                return false;
            }
        }

        public SsrsObject ReadXml(XDocument xml, string itemName, SsrsObjectPath containerPath)
        {
            try
            {
                if (xml.Root.Name == "RptDataSource")
                {
                    return ReadDataSource(xml, itemName, containerPath);
                }
                if (xml.Root.Name.LocalName == "Report" && ReportXmlSchema.IsReportNamespace(xml.Root.Name.Namespace, out _, out _))
                {
                    return ReadReport(xml, itemName, containerPath);
                }
                if (xml.Root.Name.LocalName == "SharedDataSet" && DataSetXmlSchema.IsDataSetNamespace(xml.Root.Name.Namespace, out _, out _))
                {
                    return ReadDataSet(xml, itemName, containerPath);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new SsrsObjectParseException(containerPath + itemName, ex);
            }
        }

        private SsrsDataSet ReadDataSet(XDocument xml, string itemName, SsrsObjectPath containerPath)
        {
            var dataSourceReference = new DataSetXmlSchema().GetDataSourceReference(xml);

            return new SsrsDataSet {
                Name = itemName,
                Path = containerPath + itemName,
                DataSourceReference = dataSourceReference.StartsWith("/") ? new SsrsObjectPath(dataSourceReference) : containerPath + dataSourceReference,
                Definition = new XmlObjectDefinition(xml)
            };
        }

        private SsrsReport ReadReport(XDocument xml, string itemName, SsrsObjectPath containerPath)
        {
            return new SsrsReport {
                Name = itemName,
                Path = containerPath + itemName,
                Definition = new XmlObjectDefinition(xml)
            };
        }

        private SsrsDataSource ReadDataSource(XDocument xml, string itemName, SsrsObjectPath containerPath)
        {
            var connectionString = new DataSourceXmlSchema().GetConnectionString(xml);

            return new SsrsDataSource {
                Name = itemName,
                Path = containerPath + itemName,
                ConnectionString = connectionString,
                Authentication = GetAuthenticationType()
            };

            SsrsDataSource.AuthenticationType GetAuthenticationType()
            {
                var credentials = ConnectionStringUtils.GetNetworkCredentials(connectionString);
                var integratedSecurity = new DataSourceXmlSchema().GetIntegratedSecurity(xml);

                if (credentials != null)
                {
                    return new SsrsDataSource.AuthenticationType.StoredCredentials {
                        UserName = credentials.UserName,
                        Domain = credentials.Domain,
                        Password = credentials.Password,
                        WindowsCredentials = integratedSecurity
                    };
                }
                if (integratedSecurity)
                {
                    return new SsrsDataSource.AuthenticationType.WindowsIntegrated();
                }
                return new SsrsDataSource.AuthenticationType.None();
            }
        }
    }
}
