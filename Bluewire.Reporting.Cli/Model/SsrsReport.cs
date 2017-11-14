namespace Bluewire.Reporting.Cli.Model
{
    public class SsrsReport : SsrsObject
    {
        public override SsrsObjectType Type => SsrsObjectType.Report;
        public ISsrsObjectDefinition Definition { get; set; }

        public override SsrsObject Clone()
        {
            return new SsrsReport {
                Name = Name,
                Path = Path,
                Definition = Definition
            };
        }
    }
}
