using System;
using System.Data.Common;
using Bluewire.Common.Console;

namespace Bluewire.Reporting.Cli.Support
{
    public static class ConnectionStringUtils
    {
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
