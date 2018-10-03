using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bluewire.Common.Console;
using Bluewire.Common.Console.Arguments;
using Bluewire.Common.Console.Logging;
using Bluewire.Reporting.Debugger.Configuration;

namespace Bluewire.Reporting.Debugger
{
    public class Program
    {
        static int Main(string[] args)
        {
            var session = new ConsoleSession();
            var jobFactory = SelectFactory(ref args);
            if (jobFactory == null)
            {
                Console.Error.WriteLine($@"Usage: {Path.GetFileName(session.Application)} <mode> <args...>
where <mode> is one of: dump");
                return 1;
            }
            session.Options.AddCollector(jobFactory as IReceiveOptions);
            session.ArgumentList.AddCollector(jobFactory as IReceiveArgumentList);
            var logging = session.Options.AddCollector(new SimpleConsoleLoggingPolicy());
            session.ExcessPositionalArgumentsPolicy = ExcessPositionalArgumentsPolicy.Warn;

            return session.Run(args, async () => {
                using (LoggingPolicy.Register(session, logging))
                {
                    var job = jobFactory.CreateJob();
                    await job.Run(Console.Out);
                    return 0;
                }
            });
        }

        public static IJobFactory SelectFactory(ref string[] args)
        {
            var jobType = new JobTypeParser().ParseJobTypeInPlace(ref args);
            switch (jobType)
            {
                case JobType.Dump: return new DumpReportJobFactory();
                default: return null;
            }
        }
    }
}
