using System;
using System.IO;
using Bluewire.Common.Console;
using Bluewire.Common.Console.Arguments;
using Bluewire.Reporting.Debugger.Jobs;

namespace Bluewire.Reporting.Debugger.Configuration
{
    public class DumpReportJobFactory : IJobFactory, IReceiveArgumentList
    {
        public string FilePath { get; set; }

        void IReceiveArgumentList.ReceiveFrom(ArgumentList argumentList)
        {
            argumentList.Add("file", o => FilePath = o);
        }

        public IJob CreateJob()
        {
            if (String.IsNullOrWhiteSpace(FilePath)) throw new InvalidArgumentsException("No file specified.");
            if (!File.Exists(FilePath)) throw new InvalidArgumentsException($"File does not exist: {FilePath}");
            return new DumpReportJob(FilePath);
        }
    }
}
