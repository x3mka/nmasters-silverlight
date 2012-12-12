using System;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace NMasters.Silverlight.Net.Http.Handlers
{
    public class TimeoutManager : IDisposable 
    {
        private readonly Timer _timer;

        protected TimeoutManager(IAsyncResult asyncOperationResult)
        {
            _timer = new Timer(TimeoutCallback, asyncOperationResult, TimeoutInMilliseconds, Timeout.Infinite);
        }

        static TimeoutManager()
        {
            TimeoutInMilliseconds = 100 * 1000;
        }

        public static long TimeoutInMilliseconds { get; set; }

        public static IDisposable StartCheckingForTimeout(IAsyncResult asyncOperationResult)
        {
            return new TimeoutManager(asyncOperationResult);
        }

        private static void TimeoutCallback(object context)
        {
            var asyncResult = (context as IAsyncResult);
            if (asyncResult == null)
                return;

            var requestState = asyncResult.AsyncState as HttpClientHandler.RequestState;
            if (requestState != null)
            {
                if (!asyncResult.IsCompleted)
                {
                    requestState.webRequest.Abort();
                }
            }
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }
    }
}
