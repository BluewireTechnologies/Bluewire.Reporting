namespace Bluewire.Reporting.Common.Scheduling
{
    public enum ReportPeriod
    {
        /// <summary>
        /// The report snapshot reflects the state of everything at the point in time it was created.
        /// </summary>
        Now,
        /// <summary>
        /// The report snapshot refers to a specific day.
        /// </summary>
        Day,
        /// <summary>
        /// The report snapshot refers to a specific week.
        /// </summary>
        Week,
        /// <summary>
        /// The report snapshot refers to a specific month.
        /// </summary>
        Month,
        /// <summary>
        /// The report snapshot refers to a specific year.
        /// </summary>
        Year
    }
}
