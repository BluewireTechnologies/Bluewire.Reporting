using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bluewire.Reporting.Common.Model
{
    public class SsrsObjectPath : IComparable<SsrsObjectPath>
    {
        private readonly string path;

        public SsrsObjectPath(string path)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            if (path.Contains(@"\")) throw new ArgumentException($"Object path segment separator must be '/': {path}", nameof(path));
            this.path = EnsureLeadingSlashOnly(path);
        }

        public override string ToString() => path;
        public IEnumerable<string> Segments => path.Trim('/').Split(new [] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        public bool IsRoot => path == "/";
        public string Name => Segments.Last();

        public SsrsObjectPath Parent
        {
            get
            {
                if (IsRoot) return null;
                var separatorIndex = path.LastIndexOf('/');
                var parent = path.Substring(0, separatorIndex);
                return separatorIndex == 0 ? Root : new SsrsObjectPath(parent);
            }
        }

        public static SsrsObjectPath Root => new SsrsObjectPath("/");

        public static SsrsObjectPath FromFileSystemPath(string fsPath)
        {
            if (String.IsNullOrEmpty(fsPath)) return Root;
            return new SsrsObjectPath(fsPath.Replace(Path.DirectorySeparatorChar, '/'));
        }

        public SsrsObjectPath MakeRelativeTo(SsrsObjectPath basePath)
        {
            if (!path.StartsWith(basePath.path, StringComparison.OrdinalIgnoreCase)) throw new ArgumentException($"Base path '{basePath}' is not a prefix of '{this}'", nameof(basePath));
            var remainder = path.Substring(basePath.path.Length);
            return remainder.Length == 0 ? Root : new SsrsObjectPath(remainder);
        }

        public string AsRelativeFileSystemPath() => path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);

        private static string EnsureLeadingSlashOnly(string path) => $"/{path.Trim('/')}";

        public static SsrsObjectPath operator+(SsrsObjectPath a, SsrsObjectPath b)
        {
            if (a == null) return b;
            if (b == null) return a;
            return new SsrsObjectPath(a.path + b.path);
        }

        public static SsrsObjectPath operator+(SsrsObjectPath container, string name) => container + new SsrsObjectPath(name);

        public static implicit operator string(SsrsObjectPath path) => path?.path;

        protected bool Equals(SsrsObjectPath other)
        {
            return string.Equals(path, other.path, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SsrsObjectPath) obj);
        }

        public override int GetHashCode()
        {
            return (path != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(path) : 0);
        }

        public int CompareTo(SsrsObjectPath other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(path, other.path, StringComparison.OrdinalIgnoreCase);
        }
    }
}
