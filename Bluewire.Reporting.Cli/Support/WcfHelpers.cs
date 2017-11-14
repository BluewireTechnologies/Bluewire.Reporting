using System;
using System.ServiceModel;
using log4net;

namespace Bluewire.Reporting.Cli.Support
{
    /// <summary>
    /// Helper functions for programmatically configuring WCF.
    /// </summary>
    internal static class WcfHelpers
    {
        /// <summary>
        /// Dispose, close or abort the specified object, as appropriate for its type and state.
        /// Optionally log failures.
        /// </summary>
        /// <typeparam name="T">Not actually used. Exists to disambiguate CleanUpQuietly overloads for the compiler.</typeparam>
        public static void CleanUpQuietly<T>(T obj, ILog log = null) where T : IDisposable
        {
            if (obj == null) return;
            if (obj is ICommunicationObject communicationObject)
            {
                CleanUpQuietly(communicationObject, log);
            }
            else
            {
                try
                {
                    obj.Dispose();
                }
                catch (Exception ex)
                {
                    log?.Error(ex);
                }
            }
        }

        public static void CleanUpQuietly(ICommunicationObject service, ILog log = null)
        {
            if (service == null) return;
            if (service.State != CommunicationState.Faulted)
            {
                try
                {
                    service.Close();
                }
                catch (Exception ex)
                {
                    log?.Error(ex);
                }
            }
            if (service.State == CommunicationState.Faulted)
            {
                service.Abort();
            }
        }
    }
}
