using System;
using System.IO;
using Bluewire.Common.Console;
using Bluewire.Common.Console.Arguments;
using Bluewire.Common.Console.Logging;
using Bluewire.Reporting.Cli.Configuration;

namespace Bluewire.Reporting.Cli
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
where <mode> is one of: inspect, import, create-datasource");
                return 1;
            }
            session.Options.AddCollector(jobFactory);
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
                case JobType.Inspect: return new InspectJobFactory();
                case JobType.Import: return new ImportJobFactory();
                case JobType.CreateDataSource: return new CreateDataSourceJobFactory();
                default: return null;
            }
        }
    }
}
