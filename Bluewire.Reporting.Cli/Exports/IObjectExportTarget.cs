﻿using System.Threading.Tasks;
using Bluewire.Reporting.Common.Model;

namespace Bluewire.Reporting.Cli.Exports
{
    public interface IObjectExportTarget
    {
        Task WriteObject(SsrsObject item);
    }
}
