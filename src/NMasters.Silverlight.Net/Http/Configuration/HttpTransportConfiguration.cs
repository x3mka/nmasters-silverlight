using System;
using System.Globalization;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Properties;

namespace NMasters.Silverlight.Net.Http.Configuration
{
    public class HttpTransportConfiguration
    {
        private const HttpCompletionOption DefaultCompletionOption = HttpCompletionOption.ResponseContentRead;
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30.0);
        private static readonly TimeSpan InfiniteTimeout = TimeSpan.FromMilliseconds(-1.0);
        private static readonly TimeSpan MaxTimeout = TimeSpan.FromMilliseconds(2147483647.0);        
        private const long DefaultMaxResponseContentBufferSize = 0x7fffffff;

        private TimeSpan _timeout;
        private long _maxResponseContentBufferSize;

        public HttpTransportConfiguration()
        {
            Timeout = DefaultTimeout;
            MaxResponseContentBufferSize = DefaultMaxResponseContentBufferSize;
        }

        public TimeSpan Timeout
        {
            get { return _timeout; }
            set
            {
                if ((value != InfiniteTimeout) && ((value <= TimeSpan.Zero) || (value > MaxTimeout)))
                {
                    throw new ArgumentOutOfRangeException("value");
                }                
                _timeout = value;
            }
        }

        /// <summary>Gets or sets the maximum number of bytes to buffer when reading the response content.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.The maximum number of bytes to buffer when reading the response content. The default value for this property is 64K.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The size specified is less than or equal to zero.</exception>
        /// <exception cref="T:System.InvalidOperationException">An operation has already been started on the current instance. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
        public long MaxResponseContentBufferSize
        {
            get { return _maxResponseContentBufferSize; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value > DefaultMaxResponseContentBufferSize)
                {
                    throw new ArgumentOutOfRangeException("value", string.Format(CultureInfo.InvariantCulture, SR.net_http_content_buffersize_limit, new object[] { DefaultMaxResponseContentBufferSize }));
                }                
                _maxResponseContentBufferSize = value;
            }
        }
    }
}