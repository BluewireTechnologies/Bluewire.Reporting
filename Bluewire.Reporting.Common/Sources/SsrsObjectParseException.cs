using System;
using Bluewire.Reporting.Common.Model;

namespace Bluewire.Reporting.Common.Sources
{
    public class SsrsObjectParseException : Exception
    {
        public SsrsObjectParseException(SsrsObjectPath path, Exception innerException) : base($"Failed to parse '{path}'", innerException)
        {
        }
    }
}
