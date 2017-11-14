using Bluewire.Common.Console.ThirdParty;

namespace Bluewire.Reporting.Cli.Support
{
    public static class OptionsExtensions
    {
        public static T AddCollector<T>(this OptionSet options, T collector) where T : IReceiveOptions
        {
            collector.ReceiveFrom(options);
            return collector;
        }
    }
}
