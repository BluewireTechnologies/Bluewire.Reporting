using Bluewire.Common.Console.ThirdParty;
using Bluewire.Reporting.Cli.Support;

namespace Bluewire.Reporting.Cli.Configuration
{
    public class PathOnlyFilterDefinition : IReceiveOptions
    {
        public string IncludePaths { get; set; }
        public string ExcludePaths { get; set; }

        void IReceiveOptions.ReceiveFrom(OptionSet options)
        {
            options.Add("include=", "Only include objects matching the specified path", o => IncludePaths = o);
            options.Add("exclude=", "Exclude objects matching the specified path", o => ExcludePaths = o);
        }
    }
}
