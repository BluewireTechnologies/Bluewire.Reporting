using System;
using Bluewire.Reporting.Cli.Model;
using Bluewire.Reporting.Cli.ServiceProxy;

namespace Bluewire.Reporting.Cli.Mapping
{
    public class SsrsDataSourceDefinitionMapper
    {
        public SsrsDataSource MapToSsrsObject(string itemPath, DataSourceDefinition definition)
        {
            var path = new SsrsObjectPath(itemPath);
            return new SsrsDataSource {
                Name = path.Name,
                Path = path,
                ConnectionString = definition.ConnectString,
                Authentication = MapToAuthenticationType(definition)
            };
        }

        private SsrsDataSource.AuthenticationType MapToAuthenticationType(DataSourceDefinition definition)
        {
            switch (definition.CredentialRetrieval)
            {
                case CredentialRetrievalEnum.Store:
                    var domainSeparatorIndex = definition.UserName?.IndexOf("\\") ?? -1;

                    var domain = domainSeparatorIndex > 0 ? definition.UserName?.Substring(0, domainSeparatorIndex) : null;
                    var userName = definition.UserName?.Substring(domainSeparatorIndex + 1);

                    return new SsrsDataSource.AuthenticationType.StoredCredentials {
                        Domain = domain,
                        UserName = userName,
                        Password = definition.Password,
                        WindowsCredentials = definition.WindowsCredentials
                    };
                case CredentialRetrievalEnum.Integrated:
                    return new SsrsDataSource.AuthenticationType.WindowsIntegrated();
                case CredentialRetrievalEnum.Prompt:
                    return new SsrsDataSource.AuthenticationType.Prompt();
                case CredentialRetrievalEnum.None:
                    return new SsrsDataSource.AuthenticationType.None();
            }
            throw new ArgumentOutOfRangeException($"[BUG] Unknown CredentialRetrieval enum value: {definition.CredentialRetrieval}");
        }
    }
}
