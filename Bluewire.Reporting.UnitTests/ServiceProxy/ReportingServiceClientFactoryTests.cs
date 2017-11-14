using System;
using Bluewire.Reporting.Cli.ServiceProxy;
using NUnit.Framework;

namespace Bluewire.Reporting.UnitTests.ServiceProxy
{
    [TestFixture]
    public class ReportingServiceClientFactoryTests
    {
        [TestCase("http://localhost/ReportServer/ReportService2010.asmx", "http://localhost")]
        [TestCase("http://localhost/ReportServer/ReportService2010.asmx", "http://localhost/")]
        [TestCase("http://localhost:8080/ReportServer/ReportService2010.asmx", "http://localhost:8080/")]
        [TestCase("http://localhost:8080/ReportServer2/ReportService2010.asmx", "http://localhost:8080/ReportServer2/")]
        [TestCase("http://localhost/ReportServer2/AltReportingService.ashx", "http://localhost/ReportServer2/AltReportingService.ashx")]
        [TestCase("http://localhost:8080/ReportServer2/ReportService2010.asmx", "http://localhost:8080/ReportServer2/Report")]
        public void ExpandShorthandUri(string expected, string input)
        {
            var ssrsUri = ReportingServiceClientFactory.ExpandShorthandUri(new Uri(input));
            Assert.That(ssrsUri, Is.EqualTo(new Uri(expected)));
        }
    }
}
