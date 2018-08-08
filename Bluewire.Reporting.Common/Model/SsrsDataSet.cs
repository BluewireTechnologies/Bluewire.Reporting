namespace Bluewire.Reporting.Common.Model
{
    public class SsrsDataSet : SsrsObject
    {
        public override SsrsObjectType Type => SsrsObjectType.DataSet;
        public SsrsObjectPath DataSourceReference { get; set; }
        public ISsrsObjectDefinition Definition { get; set; }

        public override SsrsObject Clone()
        {
            return new SsrsDataSet {
                Name = Name,
                Path = Path,
                DataSourceReference = DataSourceReference,
                Definition = Definition
            };
        }
    }
}
