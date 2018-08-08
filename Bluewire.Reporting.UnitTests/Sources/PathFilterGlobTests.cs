using System;
using Bluewire.Reporting.Cli.Sources;
using Bluewire.Reporting.Common.Sources;
using NUnit.Framework;

namespace Bluewire.Reporting.UnitTests.Sources
{
    [TestFixture]
    public class PathFilterGlobTests
    {
        [TestCase("*Letter", "/Test Letter")]
        [TestCase("Letter", "/Letter")]
        public void LeadingSlashIsAlwaysMatched(string pattern, string path)
        {
            var glob = PathFilter.ParseGlob(pattern);
            Assert.That(glob.Matches(path));
        }

        [TestCase("/letter*/*Y", "/Letters/Cardiology")]
        public void MatchesCaseInsensitively(string pattern, string path)
        {
            var glob = PathFilter.ParseGlob(pattern);
            Assert.That(glob.Matches(path));
        }

        // Question mark matches single characters:
        [TestCase("/Letter?/Cardiology", "/Letters/Cardiology")]
        // Single asterisk matches within a segment:
        [TestCase("/Letters/*Cardiology", "/Letters/Department of Cardiology")]
        // Double asterisk matches across segments:
        [TestCase("**Letter", "/Department/Clinic Letter")]
        [TestCase("/Letter**Cardiology", "/Letters/Departments/Cardiology")]
        public void Matches(string pattern, string path)
        {
            var glob = PathFilter.ParseGlob(pattern);
            Assert.That(glob.Matches(path));
        }

        // Question mark matches single characters, not multiple or none:
        [TestCase("/Letter?/Cardiology", "/Letter A/Cardiology")]
        [TestCase("/Letter?/Cardiology", "/Letter/Cardiology")]
        // Single asterisk matches within a segment, not across segment boundaries:
        [TestCase("/Letters/*Cardiology", "/Letters/Departments/Cardiology")]
        public void DoesNotMatch(string pattern, string path)
        {
            var glob = PathFilter.ParseGlob(pattern);
            Assert.That(glob.Matches(path), Is.False);
        }

        [TestCase("/Letter?/Cardiology", new string[0])]
        [TestCase("/Letter/Cardiology", new [] { "Letter" })]
        [TestCase("/Letters/Departments/Cardio*", new [] { "Letters", "Departments" })]
        [TestCase("/Letters/Dep*/Cardiology", new [] { "Letters" })]
        public void GetPrefixSegments(string pattern, string[] expectedPrefix)
        {
            var glob = PathFilter.ParseGlob(pattern);
            Assert.That(glob.PrefixSegments, Is.EqualTo(expectedPrefix));
        }

        [TestCase("/***/")]
        public void InvalidWildcardDoesNotParse(string pattern)
        {
            Assert.Throws<FormatException>(() => PathFilter.ParseGlob(pattern));
        }
    }
}
