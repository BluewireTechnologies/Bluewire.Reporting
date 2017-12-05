using System;
using Bluewire.Reporting.Cli.Model;
using Bluewire.Reporting.Common.ServiceProxy;

namespace Bluewire.Reporting.Cli.Mapping
{
    public class SsrsDataSourceDefinitionMapper
    {
        public void MapFromSsrsObject(DataSourceDefinition definition, SsrsDataSource item)
        {
            definition.Extension = "SQL";
            definition.ConnectString = item.ConnectionString;
            switch (item.Authentication)
            {
                case SsrsDataSource.AuthenticationType.StoredCredentials stored:
                    definition.CredentialRetrieval = CredentialRetrievalEnum.Store;
                    definition.UserName = String.IsNullOrWhiteSpace(stored.Domain) ? stored.UserName : $"{stored.Domain}\\{stored.UserName}";
                    definition.Password = stored.Password;
                    definition.WindowsCredentials = stored.WindowsCredentials;
                    break;
                case SsrsDataSource.AuthenticationType.WindowsIntegrated _:
                    definition.CredentialRetrieval = CredentialRetrievalEnum.Integrated;
                    break;
                case SsrsDataSource.AuthenticationType.Prompt _:
                    definition.CredentialRetrieval = CredentialRetrievalEnum.Prompt;
                    break;
                case SsrsDataSource.AuthenticationType.None _:
                    definition.CredentialRetrieval = CredentialRetrievalEnum.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"[BUG] Unknown authentication type: {item.Authentication.GetType()}");
            }
        }

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
