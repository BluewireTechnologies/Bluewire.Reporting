namespace Bluewire.Reporting.Common.Sources.Ssrs
{
    public enum CatalogItemType
    {
        /// <summary>
        /// A report part.
        /// </summary>
        Component,
        DataSource,
        Folder,
        Model,
        LinkedReport,
        Report,
        Resource,
        /// <summary>
        /// A shared dataset.
        /// </summary>
        DataSet,
        /// <summary>
        /// A SharePoint site.
        /// </summary>
        Site,
        /// <summary>
        /// An item not associated with any known type.
        /// </summary>
        Unknown
    }
}
