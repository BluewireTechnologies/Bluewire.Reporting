using System;

namespace Bluewire.Reporting.Cli.Model
{
    [Flags]
    public enum SsrsFilterObjectTypes
    {
        None = 0x00,
        Report = 0x01,
        DataSet = 0x02,
        DataSource = 0x04,
        All = 0x07
    }
}
