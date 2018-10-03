using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Bluewire.Common.Console;
using Bluewire.Common.Console.Arguments;
using Bluewire.Common.Console.ThirdParty;
using Bluewire.Reporting.Cli.Jobs;
using Bluewire.Reporting.Cli.ServiceProxy;
using Bluewire.Reporting.Cli.Support;
using Bluewire.Reporting.Common.Model;

namespace Bluewire.Reporting.Cli.Configuration
{
    public class CreateDataSourceJobFactory : IJobFactory, IReceiveArgumentList
    {
        public string SsrsUriString { get; set; }
        public string ObjectPath { get; set; }

        public SsrsDataSourceType Type { get; set; } = SsrsDataSourceType.SQLServer;
        public string ConnectionString { get; set; }
        public string StoreCredentials { get; set; }
        public bool UseWindowsIntegratedAuthentication { get; set; }
        public bool PromptForCredentials { get; set; }
        public bool Overwrite { get; set; }

        public ReportingServiceClientFactory ReportingServiceClientFactory { get; set; } = new ReportingServiceClientFactory();

        void IReceiveOptions.ReceiveFrom(OptionSet options)
        {
            options.Add("type=", "Type of data source (currently only SQLServer is supported)", (SsrsDataSourceType o) => Type = o);
            options.Add("connection-string=", "The connection string for the data source", o => ConnectionString = o.Unquote("'"));
            options.Add("store=", "Store credentials for this data source", o => StoreCredentials = o.Unquote("'"));
            options.Add("integrated", "Use Windows Integrated authentication", o => UseWindowsIntegratedAuthentication = true);
            options.Add("prompt", "Prompt for credentials when the data source is used", o => PromptForCredentials = true);
            options.Add("overwrite", "Replace existing objects", o => Overwrite = true);
            options.Add("timeout=", "Number of seconds to wait for SSRS webservice calls (default: 60)", (int o) => ReportingServiceClientFactory.Timeout = TimeSpan.FromSeconds(o));
        }

        void IReceiveArgumentList.ReceiveFrom(ArgumentList argumentList)
        {
            argumentList.Add("ssrs-uri", o => SsrsUriString = o);
            argumentList.Add("object-path", o => ObjectPath = o);
        }

        public IJob CreateJob()
        {
            if (String.IsNullOrWhiteSpace(SsrsUriString)) throw new InvalidArgumentsException("No SSRS URI specified.");
            if (!Uri.TryCreate(SsrsUriString, UriKind.Absolute, out var ssrsUri)) throw new InvalidArgumentsException($"Not a valid absolute URI: {SsrsUriString}");
            if (String.IsNullOrWhiteSpace(ObjectPath)) throw new InvalidArgumentsException("No object path specified.");
            if (String.IsNullOrWhiteSpace(ConnectionString)) throw new InvalidArgumentsException("No connection string specified.");

            ConnectionStringUtils.ValidateArgument<SqlConnectionStringBuilder>(ConnectionString);

            var service = ReportingServiceClientFactory.CreateFromShorthandUri(ssrsUri);
            var dataSource = BuildDataSource();
            return new CreateDataSourceJob(service, dataSource) { Overwrite = Overwrite };
        }

        private SsrsDataSource BuildDataSource()
        {
            var path = new SsrsObjectPath(ObjectPath);
            return new SsrsDataSource {
                Name = path.Name,
                Path = path,
                ConnectionString = ConnectionString,
                Authentication = BuildDataSourceAuthenticationType()
            };
        }

        private SsrsDataSource.AuthenticationType BuildDataSourceAuthenticationType()
        {
            if (StoreCredentials != null)
            {
                if (PromptForCredentials) throw new InvalidArgumentsException("Cannot specify --prompt with --store.");
                try
                {
                    var credentials = new CredentialStringParser().Parse(StoreCredentials);
                    if (!UseWindowsIntegratedAuthentication && !String.IsNullOrWhiteSpace(credentials.Domain))
                    {
                        throw new InvalidArgumentsException("Stored credentials cannot include a domain unless integrated authentication is used.");
                    }

                    return new SsrsDataSource.AuthenticationType.StoredCredentials {
                        UserName = credentials.UserName,
                        Domain = credentials.Domain,
                        Password = credentials.Password,
                        WindowsCredentials = UseWindowsIntegratedAuthentication
                    };
                }
                catch (FormatException ex)
                {
                    throw new InvalidArgumentsException(ex.Message);
                }
            }
            if (UseWindowsIntegratedAuthentication)
            {
                if (PromptForCredentials) throw new InvalidArgumentsException("Cannot specify --prompt with --integrated.");
                return new SsrsDataSource.AuthenticationType.WindowsIntegrated();
            }
            if (PromptForCredentials)
            {
                return new SsrsDataSource.AuthenticationType.Prompt();
            }
            return new SsrsDataSource.AuthenticationType.None();
        }
    }
}
