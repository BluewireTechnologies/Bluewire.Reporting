using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Bluewire.Reporting.Cli.Configuration
{
    public class CredentialStringParser
    {
        private static readonly Regex rxCredentialString = new Regex(@"^
((?<domain>[-\.\w\s]*)\\)?  (?# Optional DOMAIN\ component )
(?<userName>[-\.\w\s]*)     (?# Username component )
:                           (?# Mandatory separator )
(?<password>.*)             (?# Password component )
$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// Parse the specified string as network credentials: DOMAIN\username:password
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public NetworkCredential Parse(string str)
        {
            var m = rxCredentialString.Match(str);
            if (!m.Success) throw new FormatException("Unable to parse credentials.");

            var userName = m.Groups["userName"].Value.Trim();
            var password = m.Groups["password"].Value.Trim();

            if (String.IsNullOrWhiteSpace(userName)) throw new FormatException("No user name specified.");
            if (String.IsNullOrWhiteSpace(password)) throw new FormatException("No password specified.");

            if (m.Groups["domain"].Success)
            {
                var domain = m.Groups["domain"].Value.Trim();
                if (String.IsNullOrWhiteSpace(domain)) throw new FormatException("Empty domain specified.");
                return new NetworkCredential(userName, password, domain);
            }
            return new NetworkCredential(userName, password);
        }
    }
}
