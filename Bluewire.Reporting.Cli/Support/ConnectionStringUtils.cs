using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net;
using Bluewire.Common.Console;

namespace Bluewire.Reporting.Cli.Support
{
    public static class ConnectionStringUtils
    {
        public static NetworkCredential GetNetworkCredentials(string connectionString)
        {
            if (connectionString == null) return null;
            var builder = new SqlConnectionStringBuilder(connectionString);
            if (String.IsNullOrWhiteSpace(builder.UserID)) return null;
            return new NetworkCredential(builder.UserID, builder.Password);
        }

        public static void ValidateArgument<TBuilder>(string connectionString) where TBuilder : DbConnectionStringBuilder, new()
        {
            try
            {
                var builder = new TBuilder();
                builder.ConnectionString = connectionString;
            }
            catch (Exception ex)
            {
                throw new InvalidArgumentsException("Unable to parse connection string: {0}", ex.Message);
            }
        }
    }
}
