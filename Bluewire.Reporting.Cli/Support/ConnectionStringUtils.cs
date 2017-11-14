using System;
using System.Data.SqlClient;
using System.Net;

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
    }
}
