using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Bluewire.Reporting.Cli.Model;

namespace Bluewire.Reporting.Cli.Sources
{
    public class XmlObjectDefinition : ISsrsObjectDefinition
    {
        private readonly MemoryStream stream;

        public XmlObjectDefinition(XDocument document)
        {
            stream = new MemoryStream();
            document.Save(stream);
        }

        public Task<byte[]> GetBytes()
        {
            return Task.FromResult(stream.ToArray());
        }

        public XDocument GetXml()
        {
            stream.Position = 0;
            return XDocument.Load(stream);
        }
    }
}
