using System.IO;
using System.Threading.Tasks;

namespace Bluewire.Reporting.Debugger.Jobs
{
    public interface IJob
    {
        Task Run(TextWriter output);
    }
}
