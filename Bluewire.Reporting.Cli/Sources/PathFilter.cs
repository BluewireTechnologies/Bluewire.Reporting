using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bluewire.Reporting.Cli.Sources
{
    public static class PathFilter
    {
        public static IPathFilter MatchAll => new NoFilter(true);
        public static IPathFilter MatchNone => new NoFilter(false);

        public static IPathFilter ParseGlob(string pattern)
        {
            if (String.IsNullOrWhiteSpace(pattern)) return null;

            // Remove leading slashes (one will be added later).
            // Extract non-wildcard prefix (whole segments only).
            // Deal with special regex characters.
            // Replace glob syntax chunks with regex syntax chunks.
            // Match whole string.

            var withoutLeadingSlash = pattern.TrimStart('/');
            var escapedPattern = Regex.Escape(withoutLeadingSlash);
            var regexPattern = ReplaceGlobSyntaxWithRegexSyntax(escapedPattern);
            var regex = new Regex($"^/{regexPattern}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return new RegexFilter($"/{withoutLeadingSlash}", regex, GetPathPrefix(withoutLeadingSlash));
        }

        private static readonly Regex rxGlobWildcard = new Regex(@"((\\\*)+|\\\?)", RegexOptions.Compiled);

        private static string ReplaceGlobSyntaxWithRegexSyntax(string glob)
        {
            return rxGlobWildcard.Replace(glob, m => {
                if (m.Value == @"\*\*") return ".*";
                if (m.Value == @"\*") return "[^/]*";
                if (m.Value == @"\?") return ".";
                throw new FormatException($"Invalid wildcard token: {m.Value}");
            });
        }

        private static string[] GetPathPrefix(string pattern)
        {
            var segments = pattern.Split('/');
            // Last segment is an item name, not a container.
            var initial = segments.Take(segments.Length - 1);
            return initial.TakeWhile(s => !s.Contains('*') && !s.Contains('?')).ToArray();
        }

        struct NoFilter : IPathFilter
        {
            private readonly bool match;

            public NoFilter(bool match)
            {
                this.match = match;
            }

            public bool Matches(string path) => match;
            public IEnumerable<string> PrefixSegments => Enumerable.Empty<string>();
            public override string ToString() => match ? "**" : "";
        }

        class RegexFilter : IPathFilter
        {
            private readonly string originalPattern;
            private readonly Regex regex;

            public RegexFilter(string originalPattern, Regex regex, string[] pathPrefix)
            {
                this.originalPattern = originalPattern;
                this.regex = regex ?? throw new ArgumentNullException(nameof(regex));
                PrefixSegments = pathPrefix ?? throw new ArgumentNullException(nameof(pathPrefix));
            }

            public bool Matches(string path) => regex.IsMatch(path);
            public IEnumerable<string> PrefixSegments { get; }
            public override string ToString() => originalPattern;
        }
    }
}
