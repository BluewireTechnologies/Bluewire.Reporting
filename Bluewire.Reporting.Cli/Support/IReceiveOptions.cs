using Bluewire.Common.Console.ThirdParty;

namespace Bluewire.Reporting.Cli.Support
{
    public interface IReceiveOptions
    {
        /// <summary>
        /// Add argument-parsing information to an Options object.
        /// </summary>
        void ReceiveFrom(OptionSet options);
    }
}
