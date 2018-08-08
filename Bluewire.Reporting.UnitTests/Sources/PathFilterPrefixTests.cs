using System;
using Bluewire.Reporting.Cli.Sources;
using Bluewire.Reporting.Common.Sources;
using NUnit.Framework;

namespace Bluewire.Reporting.UnitTests.Sources
{
    [TestFixture]
    public class PathFilterPrefixTests
    {
        [TestCase("Letter", "/Letter")]
        public void LeadingSlashIsAlwaysMatched(string pattern, string path)
        {
            var glob = PathFilter.FromPrefix(pattern);
            Assert.That(glob.Matches(path));
        }

        [TestCase("/letter", "/Letters/Cardiology")]
        public void MatchesCaseInsensitively(string pattern, string path)
        {
            var glob = PathFilter.FromPrefix(pattern);
            Assert.That(glob.Matches(path));
        }

        // Question mark matches literally:
        [TestCase("/Letter?/Cardiology", "/Letter?/Cardiology")]
        [TestCase("/Letter?/Cardio", "/Letter?/Cardiology")]
        // Single asterisk matches literally:
        [TestCase("/Letters/*Cardiology", "/Letters/*Cardiology")]
        // Double asterisk matches literally:
        [TestCase("**Letter", "/**Letters/Stuff")]
        [TestCase("/Letter**Cardiology", "/Letter**Cardiology")]
        public void Matches(string pattern, string path)
        {
            var glob = PathFilter.FromPrefix(pattern);
            Assert.That(glob.Matches(path));
        }

        [TestCase("/Letter?/Cardiology", new [] { "Letter?" })]
        [TestCase("/Letter/Cardiology", new [] { "Letter" })]
        [TestCase("/Letters/Departments/Cardio*", new [] { "Letters", "Departments" })]
        [TestCase("/Letters/Dep*/Cardiology", new [] { "Letters", "Dep*" })]
        public void GetPrefixSegments(string pattern, string[] expectedPrefix)
        {
            var glob = PathFilter.FromPrefix(pattern);
            Assert.That(glob.PrefixSegments, Is.EqualTo(expectedPrefix));
        }
    }
}
