using System;
using System.IO;
using System.Linq;

namespace Bluewire.Reporting.Cli.ServiceProxy
{
    public class ReportingServiceClientFactory
    {
        public virtual IReportingServiceClient CreateFromShorthandUri(Uri shorthandUri)
        {
            return Create(ExpandShorthandUri(shorthandUri));
        }

        public virtual IReportingServiceClient Create(Uri uri)
        {
            return new ReportingServiceClient(uri);
        }

        public static Uri ExpandShorthandUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri) throw new ArgumentException($"Not an absolute URI: {uri}");
            if (uri.AbsolutePath == "/")
            {
                return new Uri(uri, "ReportServer/ReportService2010.asmx");
            }
            if (!String.IsNullOrWhiteSpace(Path.GetExtension(uri.Segments.Last())))
            {
                return uri;
            }
            return new Uri(uri, "ReportService2010.asmx");
        }
    }
}
