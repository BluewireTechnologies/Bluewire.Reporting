using Bluewire.Reporting.Common.Model;
using NUnit.Framework;

namespace Bluewire.Reporting.UnitTests.Model
{
    [TestFixture]
    public class SsrsObjectPathTests
    {
        [TestCase("/", new string[0])]
        [TestCase("/Reports", new [] { "Reports" })]
        [TestCase("/Reports/Letters", new [] { "Reports", "Letters" })]
        [TestCase("/Reports/Letters/", new [] { "Reports", "Letters" })]
        public void GetSegments(string path, string[] expectedSegments)
        {
            Assert.That(new SsrsObjectPath(path).Segments, Is.EqualTo(expectedSegments));
        }

        [TestCase("/Reports", "/")]
        [TestCase("/Reports/Letters", "/Reports")]
        [TestCase("/Reports/Letters/", "/Reports")]
        public void GetParent(string path, string expectedParent)
        {
            Assert.That(new SsrsObjectPath(path).Parent, Is.EqualTo(new SsrsObjectPath(expectedParent)));
        }

        [Test]
        public void ParentOfRootIsNull()
        {
            Assert.That(SsrsObjectPath.Root.Parent, Is.Null);
        }

        [TestCase("Reports", "/Reports")]
        [TestCase(@"Reports\Letters", "/Reports/Letters")]
        public void FromFileSystemPath(string path, string expectedPath)
        {
            Assert.That(SsrsObjectPath.FromFileSystemPath(path), Is.EqualTo(new SsrsObjectPath(expectedPath)));
        }

        [TestCase("/A", "/B", "/A/B")]
        [TestCase("/", "/B", "/B")]
        [TestCase("/A", "/", "/A")]
        public void AddOperator(string a, string b, string expectedPath)
        {
            Assert.That(new SsrsObjectPath(a) + new SsrsObjectPath(b), Is.EqualTo(new SsrsObjectPath(expectedPath)));
        }
    }
}
