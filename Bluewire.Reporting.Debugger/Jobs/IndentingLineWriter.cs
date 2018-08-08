using System;
using System.Collections.Generic;
using System.IO;

namespace Bluewire.Reporting.Debugger.Jobs
{
    class IndentingLineWriter
    {
        private readonly TextWriter writer;
        private readonly List<IndentMarker> indents = new List<IndentMarker>();

        public IndentingLineWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public IDisposable Indent(string str = "   ") => new IndentMarker(indents, str);

        public void WriteLine(string line)
        {
            foreach (var indent in indents) indent.Write(writer);
            writer.WriteLine(line);
        }

        class IndentMarker : IDisposable
        {
            private readonly ICollection<IndentMarker> indents;
            private readonly string indent;

            public IndentMarker(ICollection<IndentMarker> indents, string indent)
            {
                this.indents = indents;
                this.indent = indent;
                indents.Add(this);
            }

            public void Dispose() => indents.Remove(this);

            public void Write(TextWriter writer) => writer.Write(indent);
        }
    }
}
