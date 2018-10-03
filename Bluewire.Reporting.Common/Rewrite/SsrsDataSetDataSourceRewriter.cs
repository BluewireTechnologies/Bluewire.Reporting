using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Bluewire.Reporting.Common.Model;
using Bluewire.Reporting.Common.Sources;
using Bluewire.Reporting.Common.Xml;
using log4net;

namespace Bluewire.Reporting.Common.Rewrite
{
    public class SsrsDataSetDataSourceReferenceRewriter : ISsrsObjectRewriter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SsrsDataSetDataSourceReferenceRewriter));
        public IPathFilter ExistingReferenceFilter { get; }
        public string ReplacementReference { get; }

        public SsrsDataSetDataSourceReferenceRewriter(IPathFilter existingReferenceFilter, string replacementReference)
        {
            ExistingReferenceFilter = existingReferenceFilter ?? throw new ArgumentNullException(nameof(existingReferenceFilter));
            ReplacementReference = replacementReference ?? throw new ArgumentNullException(nameof(replacementReference));
        }

        public async Task Rewrite(SsrsObject ssrsObject)
        {
            if (ssrsObject is SsrsDataSet dataSet)
            {
                if (!ExistingReferenceFilter.Matches(dataSet.DataSourceReference)) return;

                log.DebugFormat("Applying '{0}' to '{1}'", this, dataSet.Path);
                var reference = ResolveReference(dataSet.Path, ReplacementReference);

                var bytes = await dataSet.Definition.GetBytes();
                var xml = XDocument.Load(new MemoryStream(bytes));
                new DataSetXmlSchema().SetDataSourceReference(xml, reference);

                log.InfoFormat("Dataset '{0}': Data source '{1}' => '{2}'", dataSet.Path, dataSet.DataSourceReference, reference);
                dataSet.DataSourceReference = reference;
                dataSet.Definition = new XmlObjectDefinition(xml);
            }
        }

        public override string ToString()
        {
            return $"DataSet.DataSourceReference:{{{ExistingReferenceFilter}}}={ReplacementReference}";
        }

        private SsrsObjectPath ResolveReference(SsrsObjectPath item, string reference)
        {
            if (reference.StartsWith("/")) return new SsrsObjectPath(reference);
            return item.Parent + reference;
        }
    }
}
