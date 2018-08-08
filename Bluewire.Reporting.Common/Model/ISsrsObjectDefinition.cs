using System.Threading.Tasks;

namespace Bluewire.Reporting.Common.Model
{
    public interface ISsrsObjectDefinition
    {
        Task<byte[]> GetBytes();
    }
}
