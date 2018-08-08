using System.Threading.Tasks;
using Bluewire.Reporting.Cli.Model;

namespace Bluewire.Reporting.Cli.Rewrite
{
    public interface ISsrsObjectRewriter
    {
        Task Rewrite(SsrsObject ssrsObject);
    }
}
