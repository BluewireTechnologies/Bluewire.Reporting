using System;
using System.Diagnostics;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using Bluewire.Reporting.Cli.Support;
using System.ServiceModel.Channels;
using Bluewire.Reporting.Common.ServiceProxy;

namespace Bluewire.Reporting.Cli.ServiceProxy
{
    public class ReportingServiceClient : IReportingServiceClient
    {
        private ReportingService2010SoapClient service;

        public ReportingService2010Soap Proxy => service;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(1);

        public ReportingServiceClient(Uri ssrsUri)
        {
            var binding = GetBinding(ssrsUri);
            binding.SendTimeout = Timeout;
            binding.OpenTimeout = Timeout;
            if (binding.ReceiveTimeout < Timeout) binding.ReceiveTimeout = Timeout;
            var endpointAddress = new EndpointAddress(ssrsUri);
            service = new ReportingService2010SoapClient(binding, endpointAddress);
            Debug.Assert(service.ClientCredentials != null);
            service.ClientCredentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;
            service.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;
        }

        private static Binding GetBinding(Uri ssrsUri)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(ssrsUri.Scheme, Uri.UriSchemeHttps))
            {
                return new BasicHttpsBinding(BasicHttpsSecurityMode.Transport) {
                    MaxReceivedMessageSize = int.MaxValue,
                    Security = {
                        Transport = {
                            ClientCredentialType = HttpClientCredentialType.Windows
                        }
                    }
                };
            }
            // For HTTP or anything else, return basic binding.
            // This will cause a failure elsewhere if the scheme is not HTTP.
            return new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly) {
                MaxReceivedMessageSize = int.MaxValue,
                Security = {
                    Transport = {
                        ClientCredentialType = HttpClientCredentialType.Windows
                    }
                }
            };
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
