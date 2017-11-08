using System;
using NUnit.Framework;
using Bluewire.Reporting.Cli.Configuration;

namespace Bluewire.Reporting.UnitTests.Configuration
{
    [TestFixture]
    public class CredentialStringParserTests
    {
        [TestCase("", "Unable to parse credentials.")]
        [TestCase(@"\", "Unable to parse credentials.")]
        [TestCase(":", "No username specified.")]
        [TestCase(":password", "No username specified.")]
        [TestCase("user:", "No password specified.")]
        [TestCase(@"\user:password", "Empty domain specified.")]
        public void InvalidInput(string input, string expectedMessage)
        {
            Assert.Throws<FormatException>(() => new CredentialStringParser().Parse(input), expectedMessage);
        }

        [TestCase("user:password", "user", "password")]
        [TestCase("user :password", "user", "password")]
        [TestCase("user: password", "user", "password")]
        [TestCase(@"DOMAIN\user: password", "user", "password", "DOMAIN")]
        [TestCase(@"DOMAIN\user-name: password", "user-name", "password", "DOMAIN")]
        [TestCase(@"DOMAIN \user: password", "user", "password", "DOMAIN")]
        [TestCase(@"DOMAIN\user:p@$$w0:-d", "user", "p@$$w0:-d", "DOMAIN")]
        [TestCase(@"some-domain.com\user: password", "user", "password", "some-domain.com")]
        public void ValidInput(string input, string userName, string password, string domain = "")
        {
            var credentials = new CredentialStringParser().Parse(input);
            Assert.That(credentials.UserName, Is.EqualTo(userName));
            Assert.That(credentials.Password, Is.EqualTo(password));
            Assert.That(credentials.Domain, Is.EqualTo(domain));
        }
    }
}
