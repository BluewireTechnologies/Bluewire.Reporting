using System;
using Bluewire.Reporting.Cli.Model;

namespace Bluewire.Reporting.Cli.Sources
{
    public class SsrsObjectParseException : Exception
    {
        public SsrsObjectParseException(SsrsObjectPath path, Exception innerException) : base($"Failed to parse '{path}'", innerException)
        {
        }
    }
}
