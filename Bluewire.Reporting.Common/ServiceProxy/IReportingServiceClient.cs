using System;

namespace Bluewire.Reporting.Common.ServiceProxy
{
    public interface IReportingServiceClient : IDisposable
    {
        ReportingService2010Soap Proxy { get; }
    }
}
