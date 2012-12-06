using System;
using System.Threading;

namespace NMasters.Silverlight.Net
{
    internal static class NclUtilities
    {
        internal static bool IsFatal(Exception exception)
        {
            if (exception == null)
            {
                return false;
            }
            return (((exception is OutOfMemoryException) || (exception is StackOverflowException)) || (exception is ThreadAbortException));
        }
    }
}

