using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bluewire.Reporting.Common.Model;

namespace Bluewire.Reporting.Common.Sources
{
    public interface ISsrsObjectSource : IDisposable
    {
        Task<IEnumerable<SsrsObject>> Enumerate(SsrsObjectFilter filter);
        bool HasManifest { get; }
    }
}
