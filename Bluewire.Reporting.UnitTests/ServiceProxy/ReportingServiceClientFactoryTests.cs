using System;
using System.ServiceModel;
using Bluewire.Reporting.Cli.ServiceProxy;
using Bluewire.Reporting.Common.ServiceProxy;
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

        [Test]
        public void CanCreateProxyForHttpUri()
        {
            using (var proxy = new ReportingServiceClientFactory().CreateFromShorthandUri(new Uri("http://localhost")))
            {
                try
                {
                    proxy.Proxy.IsSSLRequired(new IsSSLRequiredRequest { TrustedUserHeader = new TrustedUserHeader() });
                }
                catch (EndpointNotFoundException)
                {
                    // We don't expect to find a running endpoint during the test. We just need to
                    // verify that the URI is accepted.
                }
                catch (ServerTooBusyException)
                {
                    // If the current machine does actually have an SSRS instance running, we might
                    // occasionally ping it too often with this test.
                }
            }
        }

        [Test]
        public void CanCreateProxyForHttpsUri()
        {
            using (var proxy = new ReportingServiceClientFactory().CreateFromShorthandUri(new Uri("https://localhost")))
            {
                try
                {
                    proxy.Proxy.IsSSLRequired(new IsSSLRequiredRequest { TrustedUserHeader = new TrustedUserHeader() });
                }
                catch (EndpointNotFoundException)
                {
                    // We don't expect to find a running endpoint during the test. We just need to
                    // verify that the URI is accepted.
                }
                catch (ServerTooBusyException)
                {
                    // If the current machine does actually have an SSRS instance running, we might
                    // occasionally ping it too often with this test.
                }
            }
        }
    }
}
