using System;

namespace Bluewire.Reporting.Cli.Scheduling
{
    public class SchedulingRuleParser
    {
        public ISsrsReportSchedulingRule Parse(string rule)
        {
            if (rule == "infer") return new SsrsInferredSchedulingRule();
            throw new FormatException($"Scheduling rule could not be parsed: '{rule}'");
        }
    }
}
