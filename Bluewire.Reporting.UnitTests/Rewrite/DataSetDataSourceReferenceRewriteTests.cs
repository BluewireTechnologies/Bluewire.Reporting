using System;
using Bluewire.Reporting.Cli.Sources;
using Bluewire.Reporting.Common.Rewrite;
using Bluewire.Reporting.Common.Sources;
using NUnit.Framework;

namespace Bluewire.Reporting.UnitTests.Rewrite
{
    [TestFixture]
    public class DataSetDataSourceReferenceRewriteTests
    {
        private SsrsDataSetDataSourceReferenceRewriter ParseRule(string ruleString)
        {
            var sut = new RewriteRuleParser();
            var rule = sut.Parse(ruleString);
            Assert.That(rule, Is.InstanceOf<SsrsDataSetDataSourceReferenceRewriter>());
            return (SsrsDataSetDataSourceReferenceRewriter)rule;
        }

        [Test]
        public void ParsesSimpleReplacementRule()
        {
            var rule = ParseRule("DataSet.DataSourceReference:/EproPat");

            Assert.That(rule.ReplacementReference, Is.EqualTo("/EproPat"));

            Assert.That(rule.ExistingReferenceFilter, Is.EqualTo(PathFilter.MatchAll));
        }

        [Test]
        public void ParsesGlobReplacementRule()
        {
            var rule = ParseRule("DataSet.DataSourceReference:{Letters/**Test}=EproPat");

            Assert.That(rule.ReplacementReference, Is.EqualTo("EproPat"));

            Assert.That(rule.ExistingReferenceFilter.Matches("/Letters/Something Test"));
            Assert.That(rule.ExistingReferenceFilter.Matches("/Letters/Departments/Test"));

            Assert.That(rule.ExistingReferenceFilter.Matches("/Documents/Departments/Test"), Is.False);
        }

        [Test]
        public void RejectsPossibleTypo()
        {
            Assert.Throws<FormatException>(
                () => new RewriteRuleParser().Parse("DataSet.DataSourceReference:{Letters/**Test=EproPat"),
                "Filter must be specified if replacement value starts with '{{'."
                );
        }
    }
}
