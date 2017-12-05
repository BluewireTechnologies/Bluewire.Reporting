using System;
using System.Linq;
using System.Xml.Linq;

namespace Bluewire.Reporting.Common.Scheduling
{
    public class EproSchedulingRules
    {
        public ReportSchedule Interpret(ReportForSchedulingDto report)
        {
            return new ReportSchedule {
                Period = GetReportPeriod(report),
                Frequency = GetReportSnapshotFrequency(report)
            };
        }

        private ReportPeriod GetReportPeriod(ReportForSchedulingDto report)
        {
            if (GetBooleanProperty(report, "Weekly") == true) return ReportPeriod.Week;
            if (report.ParameterNames.Contains("Year", StringComparer.OrdinalIgnoreCase))
            {
                if (report.ParameterNames.Contains("Month", StringComparer.OrdinalIgnoreCase))
                {
                    if (report.ParameterNames.Contains("Day", StringComparer.OrdinalIgnoreCase))
                    {
                        return ReportPeriod.Day;
                    }
                    return ReportPeriod.Month;
                }
                return ReportPeriod.Year;
            }
            return ReportPeriod.Now;
        }

        private ReportSnapshotFrequency GetReportSnapshotFrequency(ReportForSchedulingDto report)
        {
            if (GetBooleanProperty(report, "Hourly") == true) return ReportSnapshotFrequency.Hourly;
            return ReportSnapshotFrequency.Daily;
        }

        private bool? GetBooleanProperty(ReportForSchedulingDto report, string name)
        {
            if (!report.Properties.TryGetValue(name, out var stringValue)) return null;
            if (!bool.TryParse(stringValue, out var value)) return null;
            return value;
        }
    }
}
