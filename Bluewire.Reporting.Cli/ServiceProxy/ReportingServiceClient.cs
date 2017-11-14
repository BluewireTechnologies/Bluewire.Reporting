using System;
using System.Diagnostics;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using Bluewire.Reporting.Cli.Support;

namespace Bluewire.Reporting.Cli.ServiceProxy
{
    public class ReportingServiceClient : IReportingServiceClient
    {
        private ReportingService2010SoapClient service;

        public ReportingService2010Soap Proxy => service;

        public ReportingServiceClient(Uri ssrsUri)
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.None) {
                MaxReceivedMessageSize = int.MaxValue,
                Security = {
                    Mode = BasicHttpSecurityMode.TransportCredentialOnly,
                    Transport = {
                        ClientCredentialType = HttpClientCredentialType.Windows
                    }
                }
            };
            var endpointAddress = new EndpointAddress(ssrsUri);
            service = new ReportingService2010SoapClient(binding, endpointAddress);
            Debug.Assert(service.ClientCredentials != null);
            service.ClientCredentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;
            service.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;
        }

        public void Dispose()
        {
            try
            {
                WcfHelpers.CleanUpQuietly(service);
            }
            finally
            {
                service = null;
            }
        }
    }
}
