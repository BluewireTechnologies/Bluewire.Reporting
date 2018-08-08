namespace Bluewire.Reporting.Common.Model
{
    public class SsrsDataSource : SsrsObject
    {
        public override SsrsObjectType Type => SsrsObjectType.DataSource;
        public string ConnectionString { get; set; }
        public AuthenticationType Authentication { get; set; }

        public override SsrsObject Clone()
        {
            return new SsrsDataSource {
                Name = Name,
                Path = Path,
                ConnectionString = ConnectionString,
                Authentication = Authentication
            };
        }

        public abstract class AuthenticationType
        {
            public class StoredCredentials : AuthenticationType
            {
                public string Domain { get; set; }
                public string UserName { get; set; }
                public string Password { get; set; }
                public bool WindowsCredentials { get; set; }
            }
            public class WindowsIntegrated : AuthenticationType { }
            public class Prompt : AuthenticationType { }
            public class None : AuthenticationType { }
        }
    }
}
