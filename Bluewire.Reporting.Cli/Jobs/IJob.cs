using System.IO;
using System.Threading.Tasks;

namespace Bluewire.Reporting.Cli.Jobs
{
    public interface IJob
    {
        Task Run(TextWriter output);
    }
}
