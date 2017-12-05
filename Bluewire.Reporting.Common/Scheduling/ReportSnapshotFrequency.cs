namespace Bluewire.Reporting.Common.Scheduling
{
    public enum ReportSnapshotFrequency
    {
        /// <summary>
        /// Snapshots are never auto-generated.
        /// </summary>
        Manual,
        /// <summary>
        /// Snapshots are generated hourly.
        /// </summary>
        Hourly,
        /// <summary>
        /// Snapshots are generated daily.
        /// </summary>
        Daily
    }
}
