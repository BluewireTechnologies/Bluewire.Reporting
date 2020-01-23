using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Bluewire.Reporting.Debugger.Jobs
{
    public class ReportStructureInterpreter
    {
        private readonly XNamespace xmlns;

        public ReportStructureInterpreter(XNamespace xmlns)
        {
            this.xmlns = xmlns ?? throw new ArgumentNullException(nameof(xmlns));
        }

        public void Interpret(XDocument xml, TextWriter output)
        {
            var indentingWriter = new IndentingLineWriter(output);
            VisitElement(xml.Root, indentingWriter);
        }

        private void VisitElement(XElement xml, IndentingLineWriter output)
        {
            if (xml.Name == xmlns + "Rectangle")
            {
                VisitRectangle(xml, output);
            }
            else if (xml.Name == xmlns + "Textbox")
            {
                VisitTextBox(xml, output);
            }
            else if (xml.Name == xmlns + "TextRun")
            {
                VisitTextRun(xml, output);
            }
            else if (xml.Name == xmlns + "Tablix")
            {
                VisitTablix(xml, output);
            }
            else if (xml.Name == xmlns + "Chart")
            {
                VisitChart(xml, output);
            }
            else
            {
                foreach (var child in xml.Elements())
                {
                    VisitElement(child, output);
                }
            }
        }

        private void VisitRectangle(XElement xml, IndentingLineWriter output)
        {
            Describe(xml, output);
            using (output.Indent())
            {
                ShowDocumentMapLabel(xml, output);
                foreach (var child in xml.Elements())
                {
                    VisitElement(child, output);
                }
            }
        }

        private void VisitTextBox(XElement xml, IndentingLineWriter output)
        {
            Describe(xml, output);
            using (output.Indent())
            {
                foreach (var child in xml.Elements())
                {
                    VisitElement(child, output);
                }
            }
        }

        private void VisitTextRun(XElement xml, IndentingLineWriter output)
        {
            var text = GetPropertyElementValue(xml, "Value");
            if (string.IsNullOrEmpty(text)) return;
            output.WriteLine($": {text}");
        }

        private void VisitTablix(XElement xml, IndentingLineWriter output)
        {
            Describe(xml, output);
            using (output.Indent())
            {
                ShowDocumentMapLabel(xml, output);
                var dataSetName = GetPropertyElementValue(xml, "DataSetName");
                if (dataSetName != null)
                {
                    output.WriteLine($"DataSet: {dataSetName}");
                }
                WriteFilters(xml, output);
                foreach (var group in xml.Elements(xmlns + "TablixRowHierarchy").Descendants(xmlns + "Group"))
                {
                    Describe(group, output);
                    using (output.Indent())
                    foreach (var expression in group.Descendants(xmlns + "GroupExpression"))
                    {
                        output.WriteLine(expression.Value);
                    }
                }
                var sorts = xml.Elements(xmlns + "TablixRowHierarchy").Descendants(xmlns + "SortExpression").Descendants(xmlns + "Value").ToList();
                if (sorts.Any())
                {
                    output.WriteLine("Sort");
                    using (output.Indent())
                    foreach (var expression in sorts)
                    {
                        output.WriteLine(expression.Value);
                    }
                }
                var i = 1;
                foreach (var row in xml.Elements(xmlns + "TablixBody").Elements(xmlns + "TablixRows").Elements(xmlns + "TablixRow"))
                {
                    output.WriteLine($"Row {i}");
                    var cells = row.Elements(xmlns + "TablixCells").Elements(xmlns + "TablixCell").ToList();
                    if (cells.Count == 1)
                    {
                        using (output.Indent())
                        {
                            VisitElement(cells.Single(), output);
                        }
                    }
                    else
                    {
                        var j = 1;
                        foreach (var cell in cells)
                        {
                            using (output.Indent(j.ToString().PadRight(3)))
                            {
                                VisitElement(cell, output);
                            }
                            j++;
                        }
                    }
                    i++;
                }
            }
        }

        private void WriteFilters(XElement xml, IndentingLineWriter output)
        {
            var filters = xml.Elements(xmlns + "Filters").Descendants(xmlns + "Filter").ToList();
            if (!filters.Any()) return;
            using (output.Indent())
            foreach (var filter in filters)
            {
                var expression = GetPropertyElementValue(filter, "FilterExpression");
                var oper = GetPropertyElementValue(filter, "Operator");
                var values = filter.Elements(xmlns + "FilterValues").Elements(xmlns + "FilterValue").Select(v => v.Value).ToList();
                if (values.Count == 0)
                {
                    output.WriteLine($"Filter: {expression} <{oper}>");
                }
                else if (values.Count == 1)
                {
                    output.WriteLine($"Filter: {expression} <{oper}> {values.Single()}");
                }
                else
                {
                    output.WriteLine($"Filter: {expression} <{oper}> ( {String.Join(" , ", values)} )");
                }
            }
        }

        private void VisitChart(XElement xml, IndentingLineWriter output)
        {
            Describe(xml, output);
            using (output.Indent())
            {
                ShowDocumentMapLabel(xml, output);
                var dataSetName = GetPropertyElementValue(xml, "DataSetName");
                if (dataSetName != null)
                {
                    output.WriteLine($"DataSet: {dataSetName}");
                }
                WriteFilters(xml, output);
                foreach (var category in xml.Elements(xmlns + "ChartCategoryHierarchy").Descendants(xmlns + "ChartMember"))
                {
                    var label = GetPropertyElementValue(category, "Label") ?? "";
                    output.WriteLine($"Category: {label}");
                    using (output.Indent())
                    {
                        foreach (var group in category.Elements(xmlns + "Group"))
                        {
                            Describe(group, output);
                            using (output.Indent())
                            foreach (var expression in group.Descendants(xmlns + "GroupExpression"))
                            {
                                output.WriteLine(expression.Value);
                            }
                        }
                        var sorts = category.Descendants(xmlns + "SortExpression").Descendants(xmlns + "Value").ToList();
                        if (sorts.Any())
                        {
                            output.WriteLine("Sort");
                            using (output.Indent())
                            foreach (var expression in sorts)
                            {
                                output.WriteLine(expression.Value);
                            }
                        }
                    }
                }
                var axesByArea = xml.Elements(xmlns + "ChartAreas").Elements(xmlns + "ChartArea")
                    .ToDictionary(a => a.Attribute("Name")?.Value ?? "", a => new {
                        Area = a,
                        CategoryAxes = a.Elements(xmlns + "ChartCategoryAxes").Elements(xmlns + "ChartAxis").ToLookup(x => x.Attribute("Name")?.Value ?? ""),
                        ValueAxes = a.Elements(xmlns + "ChartValueAxes").Elements(xmlns + "ChartAxis").ToLookup(x => x.Attribute("Name")?.Value ?? "")
                    });
                foreach (var series in xml.Elements(xmlns + "ChartData").Descendants(xmlns + "ChartSeries"))
                {
                    Describe(series, output);
                    using (output.Indent())
                    {
                        var legend = series.Element(xmlns + "ChartItemInLegend")?.Element(xmlns + "LegendText")?.Value;
                        if (legend != null)
                        {
                            output.WriteLine($"Legend: {legend}");
                        }
                        var type = GetPropertyElementValue(series, "Type");
                        if (type != null)
                        {
                            var subtype = GetPropertyElementValue(series, "Subtype");
                            if (subtype != null)
                            {
                                output.WriteLine($"Type: {type} ({subtype})");
                            }
                            else
                            {
                                output.WriteLine($"Type: {type}");
                            }
                        }
                        if (!axesByArea.TryGetValue("Default", out var area))
                        {
                            output.WriteLine("[NO AREA DEFINITION]");
                            continue;
                        }
                        var valueAxis = area.ValueAxes[GetPropertyElementValue(series, "ValueAxisName") ?? ""].FirstOrDefault();
                        if (valueAxis != null)
                        {
                            output.WriteLine($"ValueAxis: {GetPropertyElementValue(series, "ValueAxisName")}");
                        }
                        var categoryAxis = area.CategoryAxes[GetPropertyElementValue(series, "CategoryAxisName") ?? ""].FirstOrDefault();
                        if (categoryAxis != null)
                        {
                            output.WriteLine($"CategoryAxis: {GetPropertyElementValue(series, "CategoryAxisName")}");
                        }
                        foreach (var dataPoint in series.Elements(xmlns + "ChartDataPoints").Elements(xmlns + "ChartDataPoint"))
                        {
                            var tooltip = GetPropertyElementValue(dataPoint, "ToolTip");
                            if (tooltip != null)
                            {
                                output.WriteLine($"ToolTip: {tooltip}");
                            }
                            foreach (var values in dataPoint.Elements(xmlns + "ChartDataPointValues").Elements())
                            {
                                output.WriteLine($"{values.Name.LocalName}: {values.Value}");
                            }
                        }
                    }
                }
            }
        }

        private void Describe(XElement xml, IndentingLineWriter output)
        {
            var name = xml.Attribute("Name")?.Value;
            if (string.IsNullOrEmpty(name))
            {
                output.WriteLine(xml.Name.LocalName);
            }
            else
            {
                output.WriteLine($"{xml.Name.LocalName} '{name}'");
            }
        }

        private string GetPropertyElementValue(XElement xml, string localName) => xml.Element(xmlns + localName)?.Value;

        private void ShowDocumentMapLabel(XElement xml, IndentingLineWriter output)
        {
            var documentMapLabel = xml.Element(xmlns + "DocumentMapLabel")?.Value;
            if (string.IsNullOrEmpty(documentMapLabel)) return;
            output.WriteLine($"DocumentMapLabel: {documentMapLabel}");
        }
    }
}
