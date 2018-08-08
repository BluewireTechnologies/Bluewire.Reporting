using System;
using System.Text.RegularExpressions;
using Bluewire.Reporting.Cli.Sources;

namespace Bluewire.Reporting.Cli.Rewrite
{
    public class RewriteRuleParser
    {
        private readonly Regex rxReplacementRule = new Regex(@"^
(?<target>[-\.\w]*)         (?# Replacement target )
:                           (?# Mandatory separator )
(\{(?<filter>[^\}]*)\}=)?   (?# Optional filter )
(?<replacement>.*)          (?# Replacement string )
$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public ISsrsObjectRewriter Parse(string rule)
        {
            var replacementRule = rxReplacementRule.Match(rule);
            if (replacementRule.Success)
            {
                var def = new ReplacementRuleDef {
                    Target = replacementRule.Groups["target"].Value,
                    Filter = replacementRule.Groups["filter"].Success ? replacementRule.Groups["filter"].Value : null,
                    Replacement = replacementRule.Groups["replacement"].Value
                };
                if (def.Filter == null && def.Replacement.StartsWith("{"))
                {
                    // Possible typo in the rule, eg '...:{SomePattern=Replacement'.
                    throw new FormatException("Filter must be specified if replacement value starts with '{'.");
                }
                return BuildReplacementRule(def);
            }

            throw new FormatException($"Rewrite rule could not be parsed: '{rule}'");
        }

        private ISsrsObjectRewriter BuildReplacementRule(ReplacementRuleDef def)
        {
            var filter = PathFilter.ParseGlob(def.Filter) ?? PathFilter.MatchAll;
            switch (def.Target)
            {
                case "DataSet.DataSourceReference":
                    return new SsrsDataSetDataSourceReferenceRewriter(filter, def.Replacement);

                default:
                    throw new FormatException($"Unrecognised replacement target: {def.Target}");
            }
        }

        class ReplacementRuleDef
        {
            public string Target { get; set; }
            public string Filter { get; set; }
            public string Replacement { get; set; }
        }
    }
}
