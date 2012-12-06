using System;
using System.Threading;
using System.Threading.Tasks;

namespace NMasters.Silverlight.Net.Http.Helpers
{
    internal static class HttpUtilities
    {
        internal static readonly Version DefaultVersion = new Version(1,1);
        internal static readonly byte[] EmptyByteArray = new byte[0];

        internal static Task ContinueWithStandard<T>(this Task<T> task, Action<Task<T>> continuation)
        {
            return task.ContinueWith(continuation, CancellationToken.None, (TaskContinuationOptions)0x80000, TaskScheduler.Default);
        }

        internal static Task ContinueWithStandard(this Task task, Action<Task> continuation)
        {
            return task.ContinueWith(continuation, CancellationToken.None, (TaskContinuationOptions)0x80000, TaskScheduler.Default);
        }

        internal static bool HandleFaultsAndCancelation<T>(Task task, TaskCompletionSource<T> tcs)
        {
            if (task.IsFaulted)
            {
                tcs.TrySetException(task.Exception.GetBaseException());
                return true;
            }
            if (task.IsCanceled)
            {
                tcs.TrySetCanceled();
                return true;
            }
            return false;
        }

        internal static bool IsHttpUri(Uri uri)
        {
            string scheme = uri.Scheme;
            if (string.Compare("http", scheme, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return (string.Compare("https", scheme, StringComparison.OrdinalIgnoreCase) == 0);
            }
            return true;
        }
    }
}

