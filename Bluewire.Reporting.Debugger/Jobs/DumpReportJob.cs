using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Bluewire.Common.Console;
using Bluewire.Reporting.Common.Model;
using Bluewire.Reporting.Common.Sources;

namespace Bluewire.Reporting.Debugger.Jobs
{
    public class DumpReportJob : IJob
    {
        private readonly string filePath;

        public DumpReportJob(string filePath)
        {
            this.filePath = filePath;
        }

        public async Task Run(TextWriter output)
        {
            var source = new SingleFileObjectSource(filePath, new SsrsObjectPath("/"));
            var items = await source.Enumerate(new SsrsObjectFilter { Path = PathFilter.MatchAll, ObjectTypes = SsrsFilterObjectTypes.Report });

            if (items.SingleOrDefault() is SsrsReport report)
            {
                var content = new MemoryStream(await report.Definition.GetBytes());
                var xml = XDocument.Load(content);
                new ReportStructureInterpreter(xml.Root?.Name.Namespace).Interpret(xml, output);
            }
            else
            {
                throw new InvalidArgumentsException($"Unable to load report from file: {filePath}");
            }
        }
    }
}
