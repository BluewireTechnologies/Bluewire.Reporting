using Bluewire.Reporting.Cli.Configuration;
using Bluewire.Reporting.Cli.Model;
using NUnit.Framework;

namespace Bluewire.Reporting.UnitTests.Configuration
{
    [TestFixture]
    public class SsrsObjectTypesParserTests
    {
        [Test]
        public void ValidateTypes_IdentifiesInvalidTypes()
        {
            var sut = new SsrsObjectTypesParser();

            Assert.That(sut.ValidateTypes(new [] { "dataset", "something", "notvalid" }, out var invalidTypes), Is.False);
            Assert.That(invalidTypes, Is.EqualTo("something, notvalid"));
        }

        [Test]
        public void ValidateTypes_ReturnsTrueWhenNoInvalidTypes()
        {
            var sut = new SsrsObjectTypesParser();

            Assert.That(sut.ValidateTypes(new [] { "dataset", "report" }, out _), Is.True);
        }

        [Test]
        public void GetTypeFilter_ReturnsMultipleFlags()
        {
            var sut = new SsrsObjectTypesParser();

            var filter = sut.GetTypeFilter(new [] { "dataset", "report" });

            Assert.That(filter, Is.EqualTo(SsrsFilterObjectTypes.DataSet | SsrsFilterObjectTypes.Report));
        }

        [Test]
        public void GetTypeFilter_ReturnsAllFlagsWhenNoTypesSpecified()
        {
            var sut = new SsrsObjectTypesParser();

            var filter = sut.GetTypeFilter(new string[0]);

            Assert.That(filter, Is.EqualTo(SsrsFilterObjectTypes.All));
        }
    }
}
