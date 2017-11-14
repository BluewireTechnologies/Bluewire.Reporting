using System.Linq;

namespace Bluewire.Reporting.Cli
{
    public class JobTypeParser
    {
        public JobType ParseJobTypeInPlace(ref string[] args)
        {
            if (!args.Any()) return JobType.Invalid;

            if (TryInterpretAsJobType(args.First(), out var jobType))
            {
                args = args.Skip(1).ToArray();
                return jobType;
            }
            return JobType.Invalid;
        }

        private static bool TryInterpretAsJobType(string arg, out JobType jobType)
        {
            switch (arg)
            {
                default:
                    jobType = JobType.Invalid;
                    return false;
            }
        }
    }
}
