using Bluewire.Common.Console;
using Bluewire.Reporting.Cli.Jobs;
using Bluewire.Reporting.Cli.Support;

namespace Bluewire.Reporting.Cli.Configuration
{
    public interface IJobFactory : IReceiveOptions
    {
        void ConfigureSession(ConsoleSession<IJobFactory> session);

        /// <summary>
        /// Create a job using the configuration of this factory instance.
        /// </summary>
        /// <remarks>
        /// This method is expected to throw ErrorWithExitCodeExceptions when arguments are invalid, and
        /// include specific switch names in the message.
        /// </remarks>
        IJob CreateJob();
    }
}
