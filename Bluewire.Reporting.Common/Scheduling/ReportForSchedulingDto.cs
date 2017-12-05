using System.Collections.Generic;

namespace Bluewire.Reporting.Common.Scheduling
{
    public class ReportForSchedulingDto
    {
        public string[] ParameterNames { get; set; }
        public IDictionary<string, string> Properties { get; set; }
    }
}
