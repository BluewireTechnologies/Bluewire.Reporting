using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Bluewire.Reporting.Common.ServiceProxy;
using Task = System.Threading.Tasks.Task;

namespace Bluewire.Reporting.Common.Scheduling
{
    public class ReportForSchedulingReader
    {
        /// <summary>
        /// Inspect a report in SSRS.
        /// </summary>
        public async Task<ReportForSchedulingDto> FromSsrs(IReportingServiceClient service, string path)
        {
            var getParametersResult = await service.Proxy.GetItemParametersAsync(new GetItemParametersRequest { ItemPath = path });
            var getDefinitionResult = await service.Proxy.GetItemDefinitionAsync(new GetItemDefinitionRequest { ItemPath = path });
            using (var stream = new MemoryStream(getDefinitionResult.Definition))
            {
                var xml = XDocument.Load(stream);
                return new ReportForSchedulingDto {
                    ParameterNames = getParametersResult.Parameters.Select(p => p.Name).ToArray(),
                    Properties = new RdlCustomPropertiesReader().GetPropertiesFromRdl(xml)
                };
            }
        }

        /// <summary>
        /// Inspect an RDL document on the filesystem.
        /// </summary>
        public Task<ReportForSchedulingDto> FromXml(XDocument xml)
        {
            var dto = new ReportForSchedulingDto {
                ParameterNames = new RdlReportParametersReader().GetParameterNamesFromRdl(xml),
                Properties = new RdlCustomPropertiesReader().GetPropertiesFromRdl(xml)
            };
            return Task.FromResult(dto);
        }
    }
}
