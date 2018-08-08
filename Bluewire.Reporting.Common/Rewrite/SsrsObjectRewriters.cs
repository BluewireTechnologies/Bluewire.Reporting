using System.Linq;
using System.Threading.Tasks;
using Bluewire.Reporting.Common.Model;

namespace Bluewire.Reporting.Common.Rewrite
{
    public class SsrsObjectRewriters
    {
        private readonly ISsrsObjectRewriter[] rewriters;

        public SsrsObjectRewriters(params ISsrsObjectRewriter[] rewriters)
        {
            this.rewriters = rewriters;
        }

        public async Task<T> Apply<T>(T obj) where T : SsrsObject
        {
            if (!rewriters.Any()) return obj;
            var copy = (T)obj.Clone();
            foreach (var rewriter in rewriters)
            {
                await rewriter.Rewrite(copy);
            }
            return copy;
        }
    }
}
