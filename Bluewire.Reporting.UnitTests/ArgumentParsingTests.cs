using Bluewire.Common.Console;
using Bluewire.Common.Console.ThirdParty;
using Bluewire.Reporting.Cli;
using Bluewire.Reporting.Cli.Configuration;
using Bluewire.Reporting.Cli.Support;
using NUnit.Framework;

namespace Bluewire.Reporting.UnitTests
{
    [TestFixture]
    public class ArgumentParsingTests
    {
        [TestCase("inspect", ".")]
        [TestCase("inspect", "--type=datasource,dataset", ".")]
        [TestCase("inspect", "--type=report", "--include=Weekly/**/Letters*", "https://localhost/Reports/")]
        [TestCase("inspect", "--base=Reports/", "--include=Weekly/**/Letters*", ".")]
        [TestCase("inspect", "--base=Reports/", "--include=Weekly/**/Letters*", "--exclude=**/Letters By Department", ".")]
        public void ParsesValidInspectJob(params string[] arguments)
        {
            var jobFactory = ParseAs<InspectJobFactory>(arguments);
            jobFactory.CreateJob();
        }

        [TestCase("import", "https://localhost/Reports/", ".")]
        [TestCase("import", "https://localhost/Reports/", "--type=datasource,dataset", ".")]
        [TestCase("import", "--type=report", "https://localhost/Reports/", "--include=Weekly/**/Letters*", ".")]
        [TestCase("import", "--base=Reports/", "--include=Weekly/**/Letters*", "https://localhost/Reports/", ".")]
        [TestCase("import", "https://localhost/Reports/", "--type=datasource,dataset", "--rewrite=DataSet.DataSourceReference:{**/EproPat}=/EproPat", ".")]
        public void ParsesValidImportJob(params string[] arguments)
        {
            var jobFactory = ParseAs<ImportJobFactory>(arguments);
            jobFactory.CreateJob();
        }

        [TestCase("create-datasource", "https://localhost/Reports/", "/EproPat", "--connection-string=data source=(local)")]
        [TestCase("create-datasource", "--type=SQLServer", "https://localhost/Reports/", "/EproPat", "--connection-string='data source=(local)'", "--integrated")]
        [TestCase("create-datasource", "https://localhost/Reports/", "'EproPat'", "--connection-string=data source=(local)", "--prompt")]
        [TestCase("create-datasource", "https://localhost/Reports/", "/EproPat", "--connection-string=data source=(local)", "--store='user:password'")]
        [TestCase("create-datasource", "https://localhost/Reports/", "/EproPat", "--connection-string=data source=(local)", "--store='DOMAIN\\user:password'", "--integrated")]
        [TestCase("create-datasource", "https://localhost/Reports/", "/EproPat", "--connection-string=data source=(local)", "--store=user:password", "--overwrite")]
        public void ParsesValidCreateDataSourceJob(params string[] arguments)
        {
            var jobFactory = ParseAs<CreateDataSourceJobFactory>(arguments);
            jobFactory.CreateJob();
        }

        private static T ParseAs<T>(params string[] args) where T : IJobFactory
        {
            var options = new OptionSet();
            var jobFactory = Program.SelectFactory(ref args);
            Assert.That(jobFactory, Is.Not.Null);
            Assert.That(jobFactory, Is.InstanceOf<T>());
            options.AddCollector(jobFactory);
            var session = new SessionArguments<IJobFactory>(jobFactory, options);
            session.Parse(args);
            return (T)jobFactory;
        }
    }
}
