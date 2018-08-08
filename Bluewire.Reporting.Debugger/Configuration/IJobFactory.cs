using Bluewire.Reporting.Debugger.Jobs;

namespace Bluewire.Reporting.Debugger.Configuration
{
    public interface IJobFactory
    {
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
