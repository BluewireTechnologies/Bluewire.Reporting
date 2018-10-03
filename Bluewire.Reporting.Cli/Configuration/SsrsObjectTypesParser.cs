using System;
using System.Collections.Generic;
using System.Linq;
using Bluewire.Reporting.Common.Model;

namespace Bluewire.Reporting.Cli.Configuration
{
    public class SsrsObjectTypesParser
    {
        public SsrsFilterObjectTypes GetTypeFilter(ICollection<string> types)
        {
            if (!types.Any()) return SsrsFilterObjectTypes.All;
            var filter = SsrsFilterObjectTypes.None;
            foreach (var typeName in types)
            {
                if (!TryParse(typeName, out var type))
                {
                    throw new ArgumentException($"Invalid object type: {typeName}");
                }
                filter |= type;
            }
            return filter;
        }

        public bool ValidateTypes(ICollection<string> types, out string invalidTypes)
        {
            invalidTypes = String.Join(", ", types.Where(t => !TryParse(t, out _)));
            return String.IsNullOrWhiteSpace(invalidTypes);
        }

        public bool TryParse(string typeName, out SsrsFilterObjectTypes type) => Enum.TryParse(typeName, true, out type);
    }
}
