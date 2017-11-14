using System;
using Bluewire.Common.Console;
using Bluewire.Common.Console.ThirdParty;
using Bluewire.Reporting.Cli.Support;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace Bluewire.Reporting.Cli.Configuration
{
    public class Logging : IReceiveOptions
    {
        private readonly VerbosityList<Level> verbosity = new VerbosityList<Level>(Level.Error, Level.Warn, Level.Info, Level.Debug).Default(Level.Warn);

        public void Configure()
        {
            BasicConfigurator.Configure(
                new TextWriterAppender {
                    Writer = Console.Error,
                    Layout = new PatternLayout("%message%newline"),
                    Threshold = verbosity.CurrentVerbosity
                });
        }

        void IReceiveOptions.ReceiveFrom(OptionSet options)
        {
            options.Add("v|verbose", "Verbose mode", o => verbosity.Verbose());
            options.Add("q|quiet", "Quiet mode", o => verbosity.Quiet());
        }
    }
}
