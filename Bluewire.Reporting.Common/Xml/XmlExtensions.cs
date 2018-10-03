using System;
using System.Xml.Linq;

namespace Bluewire.Reporting.Common.Xml
{
    public static class XmlExtensions
    {
        public static bool GetBoolean(this XElement element) => StringComparer.OrdinalIgnoreCase.Equals(element.Value, "true");
    }
}
