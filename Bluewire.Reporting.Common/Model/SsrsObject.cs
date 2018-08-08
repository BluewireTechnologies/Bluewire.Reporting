namespace Bluewire.Reporting.Common.Model
{
    public abstract class SsrsObject
    {
        public string Name { get; set; }
        public SsrsObjectPath Path { get; set; }
        public abstract SsrsObjectType Type { get; }
        public abstract SsrsObject Clone();
    }
}
