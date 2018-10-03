using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bluewire.Reporting.Cli.Sources;
using log4net;
using System.Data.SqlClient;
using Bluewire.Reporting.Common.Model;
using Bluewire.Reporting.Common.Sources;

namespace Bluewire.Reporting.Cli.Jobs
{
    public class InspectJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InspectJob));
        private readonly ISsrsObjectSource source;
        private readonly SsrsObjectFilter filter;

        public InspectJob(
            ISsrsObjectSource source,
            SsrsObjectFilter filter
            )
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public async Task Run(TextWriter output)
        {
            log.Info("Collecting objects...");
            var items = await source.Enumerate(filter);
            foreach (var item in items.OrderBy(i => i.Path))
            {
                output.WriteLine($"Path:   {item.Path}");
                output.WriteLine($"Type:   {item.Type}");
                if (item is SsrsDataSource dataSource) DescribeDataSource(output, dataSource);
                if (item is SsrsDataSet dataSet) DescribeDataSet(output, dataSet);
                if (item is SsrsReport report) DescribeReport(output, report);
                output.WriteLine();
            }
        }

        private void DescribeDataSource(TextWriter output, SsrsDataSource dataSource)
        {
            output.WriteLine($"Connection string:   {HideConnectionStringPassword(dataSource.ConnectionString)}");

            switch (dataSource.Authentication)
            {
                case SsrsDataSource.AuthenticationType.StoredCredentials stored:
                    output.WriteLine( "Authentication:      Stored credentials");
                    if (stored.WindowsCredentials)
                    {
                        output.WriteLine($"    Domain:          {stored.Domain ?? ""}");
                    }
                    output.WriteLine($"    User Name:       {stored.UserName}");
                    output.WriteLine( "    Password:        <hidden>");
                    break;
                case SsrsDataSource.AuthenticationType.WindowsIntegrated _:
                    output.WriteLine( "Authentication:      Windows Integrated");
                    break;
                case SsrsDataSource.AuthenticationType.Prompt _:
                    output.WriteLine( "Authentication:      Prompt for credentials");
                    break;
                case SsrsDataSource.AuthenticationType.None _:
                    output.WriteLine( "Authentication:      None");
                    break;
            }
        }

        private string HideConnectionStringPassword(string connectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                if (String.IsNullOrWhiteSpace(builder.Password)) return connectionString;
                builder.Password = "<hidden>";
                return builder.ConnectionString;
            }
            catch
            {
                return connectionString;
            }
        }

        private void DescribeDataSet(TextWriter output, SsrsDataSet dataSet)
        {
            output.WriteLine($"Data Source:     {dataSet.DataSourceReference}");
        }

        private void DescribeReport(TextWriter output, SsrsReport report)
        {
        }
    }
}
