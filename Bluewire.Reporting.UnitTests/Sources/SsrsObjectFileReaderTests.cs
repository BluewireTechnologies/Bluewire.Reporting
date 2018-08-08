using System.Xml.Linq;
using Bluewire.Reporting.Cli.Sources;
using Bluewire.Reporting.Common.Model;
using Bluewire.Reporting.Common.Sources;
using NUnit.Framework;

namespace Bluewire.Reporting.UnitTests.Sources
{
    [TestFixture]
    public class SsrsObjectFileReaderTests
    {
        [Test]
        public void CanRead2010Report()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition"" xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"">
</Report>";

            var report = new SsrsObjectFileReader().ReadXml(XDocument.Parse(xml), "Test", new SsrsObjectPath("Test"));

            Assert.That(report, Is.InstanceOf<SsrsReport>());
        }

        [Test]
        public void CanRead2016Report()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition"" xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"">
</Report>";

            var report = new SsrsObjectFileReader().ReadXml(XDocument.Parse(xml), "Test", new SsrsObjectPath("Test"));

            Assert.That(report, Is.InstanceOf<SsrsReport>());
        }

        [Test]
        public void CanRead2010DataSet()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<SharedDataSet xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/shareddatasetdefinition"">
  <DataSet Name="""">
    <Query>
      <DataSourceReference>DataSourceName</DataSourceReference>
    </Query>
  </DataSet>
</SharedDataSet>";

            var report = new SsrsObjectFileReader().ReadXml(XDocument.Parse(xml), "Test", new SsrsObjectPath("Test"));

            Assert.That(report, Is.InstanceOf<SsrsDataSet>());
        }

        [Test]
        public void CanRead2016DataSet()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<SharedDataSet xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2016/01/shareddatasetdefinition"">
  <DataSet Name="""">
    <Query>
      <DataSourceReference>DataSourceName</DataSourceReference>
    </Query>
  </DataSet>
</SharedDataSet>";

            var report = new SsrsObjectFileReader().ReadXml(XDocument.Parse(xml), "Test", new SsrsObjectPath("Test"));

            Assert.That(report, Is.InstanceOf<SsrsDataSet>());
        }
    }
}
