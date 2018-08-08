using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bluewire.Reporting.Common.Model;

namespace Bluewire.Reporting.Common.Sources
{
    public class AggregatedObjectSource : ISsrsObjectSource
    {
        private readonly List<ISsrsObjectSource> sources = new List<ISsrsObjectSource>();
        private ISsrsObjectSource[] GetSources() => sources.ToArray();

        public void Add(ISsrsObjectSource source) => sources.Add(source);

        public async Task<IEnumerable<SsrsObject>> Enumerate(SsrsObjectFilter filter)
        {
            var enumerables = await Task.WhenAll(GetSources().Select(s => s.Enumerate(filter)));
            return enumerables.SelectMany(i => i);
        }

        public bool HasManifest => GetSources().Any(s => s.HasManifest);

        public void Dispose()
        {
            var disposables = GetSources();
            sources.Clear();
            foreach (var disposable in disposables) disposable.Dispose();
        }
    }
}
