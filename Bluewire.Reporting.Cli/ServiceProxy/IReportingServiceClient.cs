using System;

namespace Bluewire.Reporting.Cli.ServiceProxy
{
    public interface IReportingServiceClient : IDisposable
    {
        ReportingService2010Soap Proxy { get; }
    }
}
