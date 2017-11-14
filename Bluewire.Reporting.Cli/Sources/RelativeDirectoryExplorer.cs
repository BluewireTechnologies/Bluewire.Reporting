using System.Collections.Generic;
using System.IO;

namespace Bluewire.Reporting.Cli.Sources
{
    class RelativeDirectoryExplorer
    {
        public IEnumerable<string> EnumerateRelativeFiles(DirectoryInfo container)
        {
            var queue = new Queue<Item>();
            queue.Enqueue(new Item { Container = container, RelativePath = "" });
            return EnumerateRelativeFiles(queue);
        }

        private IEnumerable<string> EnumerateRelativeFiles(Queue<Item> queue)
        {
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var file in current.Container.EnumerateFiles())
                {
                    yield return Path.Combine(current.RelativePath, file.Name);
                }
                foreach (var directory in current.Container.EnumerateDirectories())
                {
                    queue.Enqueue(new Item { Container = directory, RelativePath = Path.Combine(current.RelativePath, directory.Name) });
                }
            }
        }

        struct Item
        {
            public DirectoryInfo Container { get; set; }
            public string RelativePath { get; set; }
        }
    }
}
