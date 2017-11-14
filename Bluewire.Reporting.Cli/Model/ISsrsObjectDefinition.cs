using System.Threading.Tasks;

namespace Bluewire.Reporting.Cli.Model
{
    public interface ISsrsObjectDefinition
    {
        Task<byte[]> GetBytes();
    }
}
