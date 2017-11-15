using System;
using System.IO;
using System.Reflection;
using Bluewire.Common.Console;
using Bluewire.Common.Console.ThirdParty;
using Bluewire.Reporting.Cli.Configuration;
using Bluewire.Reporting.Cli.Support;

namespace Bluewire.Reporting.Cli
{
    public class Program
    {
        static int Main(string[] args)
        {
            var options = new OptionSet();
            var jobFactory = SelectFactory(ref args);
            if (jobFactory == null)
            {
                Console.Error.WriteLine($@"Usage: {Path.GetFileName(Assembly.GetEntryAssembly().Location)} <mode> <args...>
where <mode> is one of: inspect, import");
                return 1;
            }
            options.AddCollector(jobFactory);
            var logging = options.AddCollector(new Logging());
            var session = new ConsoleSession<IJobFactory>(jobFactory, options);
            jobFactory.ConfigureSession(session);

            return session.Run(args, async f => {
                logging.Configure();
                var job = f.CreateJob();
                await job.Run(Console.Out);
                return 0;
            });
        }

        public static IJobFactory SelectFactory(ref string[] args)
        {
            var jobType = new JobTypeParser().ParseJobTypeInPlace(ref args);
            switch (jobType)
            {
                case JobType.Inspect: return new InspectJobFactory();
                case JobType.Import: return new ImportJobFactory();
                default: return null;
            }
        }
    }
}
