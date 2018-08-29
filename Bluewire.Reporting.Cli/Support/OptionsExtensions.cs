using Bluewire.Common.Console.ThirdParty;

namespace Bluewire.Reporting.Cli.Support
{
    public static class OptionsExtensions
    {
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
