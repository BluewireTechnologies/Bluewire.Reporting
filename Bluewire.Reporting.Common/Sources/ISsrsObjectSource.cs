using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bluewire.Reporting.Cli.Model;

namespace Bluewire.Reporting.Cli.Sources
{
    public interface ISsrsObjectSource : IDisposable
    {
        Task<IEnumerable<SsrsObject>> Enumerate(SsrsObjectFilter filter);
        bool HasManifest { get; }
    }
}
