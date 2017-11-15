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

        public static string Unquote(this string str, params string[] quotes)
        {
            foreach (var quote in quotes)
            {
                if (!str.StartsWith(quote)) continue;
                if (!str.EndsWith(quote)) break;
                return str.Substring(quote.Length, str.Length - quote.Length - quote.Length);
            }
            return str;
        }
    }
}
