using System.Threading.Tasks;
using Bluewire.Reporting.Common.Model;

namespace Bluewire.Reporting.Common.Rewrite
{
    public interface ISsrsObjectRewriter
    {
        Task Rewrite(SsrsObject ssrsObject);
    }
}
