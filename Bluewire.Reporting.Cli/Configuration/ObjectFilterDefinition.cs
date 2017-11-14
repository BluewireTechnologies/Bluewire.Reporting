using Bluewire.Common.Console.ThirdParty;
using Bluewire.Reporting.Cli.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bluewire.Reporting.Cli.Configuration
{
    public class ObjectFilterDefinition : IReceiveOptions
    {
        public string IncludePaths { get; set; }
        public string ExcludePaths { get; set; }
        public ISet<string> TypeFilter { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void IReceiveOptions.ReceiveFrom(OptionSet options)
        {
            options.Add("include=", "Only include objects matching the specified path", o => IncludePaths = o);
            options.Add("exclude=", "Exclude objects matching the specified path", o => ExcludePaths = o);
            options.Add("type=", "Only include objects matching the specified type(s)", o => TypeFilter.UnionWith(o.Split(',').Select(t => t.Trim())));
        }
    }
}
