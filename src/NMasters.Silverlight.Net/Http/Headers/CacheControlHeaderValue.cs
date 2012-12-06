using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents the value of the Cache-Control header.</summary>
    public class CacheControlHeaderValue //: ICloneable
    {
        private static readonly Action<string> checkIsValidToken = new Action<string>(CacheControlHeaderValue.CheckIsValidToken);
        private ICollection<NameValueHeaderValue> extensions;
        private TimeSpan? maxAge;
        private const string maxAgeString = "max-age";
        private bool maxStale;
        private TimeSpan? maxStaleLimit;
        private const string maxStaleString = "max-stale";
        private TimeSpan? minFresh;
        private const string minFreshString = "min-fresh";
        private bool mustRevalidate;
        private const string mustRevalidateString = "must-revalidate";
        private static readonly HttpHeaderParser nameValueListParser = GenericHeaderParser.MultipleValueNameValueParser;
        private bool noCache;
        private ICollection<string> noCacheHeaders;
        private const string noCacheString = "no-cache";
        private bool noStore;
        private const string noStoreString = "no-store";
        private bool noTransform;
        private const string noTransformString = "no-transform";
        private bool onlyIfCached;
        private const string onlyIfCachedString = "only-if-cached";
        private bool privateField;
        private ICollection<string> privateHeaders;
        private const string privateString = "private";
        private bool proxyRevalidate;
        private const string proxyRevalidateString = "proxy-revalidate";
        private bool publicField;
        private const string publicString = "public";
        private TimeSpan? sharedMaxAge;
        private const string sharedMaxAgeString = "s-maxage";

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.CacheControlHeaderValue" /> class.</summary>
        public CacheControlHeaderValue()
        {
        }

        private CacheControlHeaderValue(CacheControlHeaderValue source)
        {
            this.noCache = source.noCache;
            this.noStore = source.noStore;
            this.maxAge = source.maxAge;
            this.sharedMaxAge = source.sharedMaxAge;
            this.maxStale = source.maxStale;
            this.maxStaleLimit = source.maxStaleLimit;
            this.minFresh = source.minFresh;
            this.noTransform = source.noTransform;
            this.onlyIfCached = source.onlyIfCached;
            this.publicField = source.publicField;
            this.privateField = source.privateField;
            this.mustRevalidate = source.mustRevalidate;
            this.proxyRevalidate = source.proxyRevalidate;
            if (source.noCacheHeaders != null)
            {
                foreach (string str in source.noCacheHeaders)
                {
                    this.NoCacheHeaders.Add(str);
                }
            }
            if (source.privateHeaders != null)
            {
                foreach (string str2 in source.privateHeaders)
                {
                    this.PrivateHeaders.Add(str2);
                }
            }
            // SL fixes
            //if (source.extensions != null)
            //{
            //    foreach (NameValueHeaderValue value2 in source.extensions)
            //    {
            //        this.Extensions.Add((NameValueHeaderValue) ((ICloneable) value2).Clone());
            //    }
            //}
        }

        private static void AppendValueIfRequired(StringBuilder sb, bool appendValue, string value)
        {
            if (appendValue)
            {
                AppendValueWithSeparatorIfRequired(sb, value);
            }
        }

        private static void AppendValues(StringBuilder sb, IEnumerable<string> values)
        {
            bool flag = true;
            foreach (string str in values)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    sb.Append(", ");
                }
                sb.Append(str);
            }
        }

        private static void AppendValueWithSeparatorIfRequired(StringBuilder sb, string value)
        {
            if (sb.Length > 0)
            {
                sb.Append(", ");
            }
            sb.Append(value);
        }

        private static void CheckIsValidToken(string item)
        {
            HeaderUtilities.CheckValidToken(item, "item");
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.CacheControlHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            CacheControlHeaderValue value2 = obj as CacheControlHeaderValue;
            if ((((((value2 == null) || ((this.noCache != value2.noCache) || (this.noStore != value2.noStore))) || (this.maxAge != value2.maxAge)) || ((this.sharedMaxAge != value2.sharedMaxAge) || (this.maxStale != value2.maxStale))) || (this.maxStaleLimit != value2.maxStaleLimit)) || ((((this.minFresh != value2.minFresh) || (this.noTransform != value2.noTransform)) || ((this.onlyIfCached != value2.onlyIfCached) || (this.publicField != value2.publicField))) || (((this.privateField != value2.privateField) || (this.mustRevalidate != value2.mustRevalidate)) || (this.proxyRevalidate != value2.proxyRevalidate))))
            {
                return false;
            }
            if (!HeaderUtilities.AreEqualCollections<string>(this.noCacheHeaders, value2.noCacheHeaders, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!HeaderUtilities.AreEqualCollections<string>(this.privateHeaders, value2.privateHeaders, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this.extensions, value2.extensions))
            {
                return false;
            }
            return true;
        }

        internal static int GetCacheControlLength(string input, int startIndex, CacheControlHeaderValue storeValue, out CacheControlHeaderValue parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            int index = startIndex;
            object obj2 = null;
            List<NameValueHeaderValue> nameValueList = new List<NameValueHeaderValue>();
            while (index < input.Length)
            {
                if (!nameValueListParser.TryParseValue(input, null, ref index, out obj2))
                {
                    return 0;
                }
                nameValueList.Add(obj2 as NameValueHeaderValue);
            }
            CacheControlHeaderValue cc = storeValue;
            if (cc == null)
            {
                cc = new CacheControlHeaderValue();
            }
            if (!TrySetCacheControlValues(cc, nameValueList))
            {
                return 0;
            }
            if (storeValue == null)
            {
                parsedValue = cc;
            }
            return (input.Length - startIndex);
        }

        /// <summary>Serves as a hash function for a  <see cref="T:NMasters.Silverlight.Net.Http.Headers.CacheControlHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int num = (((((((this.noCache.GetHashCode() ^ (this.noStore.GetHashCode() << 1)) ^ (this.maxStale.GetHashCode() << 2)) ^ (this.noTransform.GetHashCode() << 3)) ^ (this.onlyIfCached.GetHashCode() << 4)) ^ (this.publicField.GetHashCode() << 5)) ^ (this.privateField.GetHashCode() << 6)) ^ (this.mustRevalidate.GetHashCode() << 7)) ^ (this.proxyRevalidate.GetHashCode() << 8);
            num = (((num ^ (this.maxAge.HasValue ? (this.maxAge.Value.GetHashCode() ^ 1) : 0)) ^ (this.sharedMaxAge.HasValue ? (this.sharedMaxAge.Value.GetHashCode() ^ 2) : 0)) ^ (this.maxStaleLimit.HasValue ? (this.maxStaleLimit.Value.GetHashCode() ^ 4) : 0)) ^ (this.minFresh.HasValue ? (this.minFresh.Value.GetHashCode() ^ 8) : 0);
            if ((this.noCacheHeaders != null) && (this.noCacheHeaders.Count > 0))
            {
                foreach (string str in this.noCacheHeaders)
                {
                    num ^= str.ToLowerInvariant().GetHashCode();
                }
            }
            if ((this.privateHeaders != null) && (this.privateHeaders.Count > 0))
            {
                foreach (string str2 in this.privateHeaders)
                {
                    num ^= str2.ToLowerInvariant().GetHashCode();
                }
            }
            if ((this.extensions != null) && (this.extensions.Count > 0))
            {
                foreach (NameValueHeaderValue value2 in this.extensions)
                {
                    num ^= value2.GetHashCode();
                }
            }
            return num;
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.CacheControlHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.CacheControlHeaderValue" />.A <see cref="T:NMasters.Silverlight.Net.Http.Headers.CacheControlHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents cache-control header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid cache-control header value information.</exception>
        public static CacheControlHeaderValue Parse(string input)
        {
            int index = 0;
            return (CacheControlHeaderValue) CacheControlHeaderParser.Parser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.CacheControlHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new CacheControlHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.CacheControlHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendValueIfRequired(sb, this.noStore, "no-store");
            AppendValueIfRequired(sb, this.noTransform, "no-transform");
            AppendValueIfRequired(sb, this.onlyIfCached, "only-if-cached");
            AppendValueIfRequired(sb, this.publicField, "public");
            AppendValueIfRequired(sb, this.mustRevalidate, "must-revalidate");
            AppendValueIfRequired(sb, this.proxyRevalidate, "proxy-revalidate");
            if (this.noCache)
            {
                AppendValueWithSeparatorIfRequired(sb, "no-cache");
                if ((this.noCacheHeaders != null) && (this.noCacheHeaders.Count > 0))
                {
                    sb.Append("=\"");
                    AppendValues(sb, this.noCacheHeaders);
                    sb.Append('"');
                }
            }
            if (this.maxAge.HasValue)
            {
                AppendValueWithSeparatorIfRequired(sb, "max-age");
                sb.Append('=');
                sb.Append(((int) this.maxAge.Value.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
            }
            if (this.sharedMaxAge.HasValue)
            {
                AppendValueWithSeparatorIfRequired(sb, "s-maxage");
                sb.Append('=');
                sb.Append(((int) this.sharedMaxAge.Value.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
            }
            if (this.maxStale)
            {
                AppendValueWithSeparatorIfRequired(sb, "max-stale");
                if (this.maxStaleLimit.HasValue)
                {
                    sb.Append('=');
                    sb.Append(((int) this.maxStaleLimit.Value.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
                }
            }
            if (this.minFresh.HasValue)
            {
                AppendValueWithSeparatorIfRequired(sb, "min-fresh");
                sb.Append('=');
                sb.Append(((int) this.minFresh.Value.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
            }
            if (this.privateField)
            {
                AppendValueWithSeparatorIfRequired(sb, "private");
                if ((this.privateHeaders != null) && (this.privateHeaders.Count > 0))
                {
                    sb.Append("=\"");
                    AppendValues(sb, this.privateHeaders);
                    sb.Append('"');
                }
            }
            NameValueHeaderValue.ToString(this.extensions, ',', false, sb);
            return sb.ToString();
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.CacheControlHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.CacheControlHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.CacheControlHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out CacheControlHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (CacheControlHeaderParser.Parser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (CacheControlHeaderValue) obj2;
                return true;
            }
            return false;
        }

        private static bool TrySetCacheControlValues(CacheControlHeaderValue cc, List<NameValueHeaderValue> nameValueList)
        {
            foreach (NameValueHeaderValue value2 in nameValueList)
            {
                bool flag = true;
                switch (value2.Name.ToLowerInvariant())
                {
                    case "no-cache":
                        flag = TrySetOptionalTokenList(value2, ref cc.noCache, ref cc.noCacheHeaders);
                        break;

                    case "no-store":
                        flag = TrySetTokenOnlyValue(value2, ref cc.noStore);
                        break;

                    case "max-age":
                        flag = TrySetTimeSpan(value2, ref cc.maxAge);
                        break;

                    case "max-stale":
                        flag = (value2.Value == null) || TrySetTimeSpan(value2, ref cc.maxStaleLimit);
                        if (flag)
                        {
                            cc.maxStale = true;
                        }
                        break;

                    case "min-fresh":
                        flag = TrySetTimeSpan(value2, ref cc.minFresh);
                        break;

                    case "no-transform":
                        flag = TrySetTokenOnlyValue(value2, ref cc.noTransform);
                        break;

                    case "only-if-cached":
                        flag = TrySetTokenOnlyValue(value2, ref cc.onlyIfCached);
                        break;

                    case "public":
                        flag = TrySetTokenOnlyValue(value2, ref cc.publicField);
                        break;

                    case "private":
                        flag = TrySetOptionalTokenList(value2, ref cc.privateField, ref cc.privateHeaders);
                        break;

                    case "must-revalidate":
                        flag = TrySetTokenOnlyValue(value2, ref cc.mustRevalidate);
                        break;

                    case "proxy-revalidate":
                        flag = TrySetTokenOnlyValue(value2, ref cc.proxyRevalidate);
                        break;

                    case "s-maxage":
                        flag = TrySetTimeSpan(value2, ref cc.sharedMaxAge);
                        break;

                    default:
                        cc.Extensions.Add(value2);
                        break;
                }
                if (!flag)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool TrySetOptionalTokenList(NameValueHeaderValue nameValue, ref bool boolField, ref ICollection<string> destination)
        {
            if (nameValue.Value == null)
            {
                boolField = true;
                return true;
            }
            string input = nameValue.Value;
            if (((input.Length >= 3) && (input[0] == '"')) && (input[input.Length - 1] == '"'))
            {
                int startIndex = 1;
                int num2 = input.Length - 1;
                bool separatorFound = false;
                int num3 = (destination == null) ? 0 : destination.Count;
                while (startIndex < num2)
                {
                    startIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex, true, out separatorFound);
                    if (startIndex == num2)
                    {
                        break;
                    }
                    int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
                    if (tokenLength == 0)
                    {
                        return false;
                    }
                    if (destination == null)
                    {
                        destination = new ObjectCollection<string>(checkIsValidToken);
                    }
                    destination.Add(input.Substring(startIndex, tokenLength));
                    startIndex += tokenLength;
                }
                if ((destination != null) && (destination.Count > num3))
                {
                    boolField = true;
                    return true;
                }
            }
            return false;
        }

        private static bool TrySetTimeSpan(NameValueHeaderValue nameValue, ref TimeSpan? timeSpan)
        {
            int num;
            if (nameValue.Value == null)
            {
                return false;
            }
            if (!HeaderUtilities.TryParseInt32(nameValue.Value, out num))
            {
                return false;
            }
            timeSpan = new TimeSpan(0, 0, num);
            return true;
        }

        private static bool TrySetTokenOnlyValue(NameValueHeaderValue nameValue, ref bool boolField)
        {
            if (nameValue.Value != null)
            {
                return false;
            }
            boolField = true;
            return true;
        }

        /// <summary>Cache-extension tokens, each with an optional assigned value.</summary>
        /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.A collection of cache-extension tokens each with an optional assigned value.</returns>
        public ICollection<NameValueHeaderValue> Extensions
        {
            get
            {
                if (this.extensions == null)
                {
                    this.extensions = new ObjectCollection<NameValueHeaderValue>();
                }
                return this.extensions;
            }
        }

        /// <summary>The maximum age, specified in seconds, that the HTTP client is willing to accept a response. </summary>
        /// <returns>Returns <see cref="T:System.TimeSpan" />.The time in seconds. </returns>
        public TimeSpan? MaxAge
        {
            get
            {
                return this.maxAge;
            }
            set
            {
                this.maxAge = value;
            }
        }

        /// <summary>Whether an HTTP client is willing to accept a response that has exceeded its expiration time.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the HTTP client is willing to accept a response that has exceed the expiration time; otherwise, false.</returns>
        public bool MaxStale
        {
            get
            {
                return this.maxStale;
            }
            set
            {
                this.maxStale = value;
            }
        }

        /// <summary>The maximum time, in seconds, an HTTP client is willing to accept a response that has exceeded its expiration time.</summary>
        /// <returns>Returns <see cref="T:System.TimeSpan" />.The time in seconds.</returns>
        public TimeSpan? MaxStaleLimit
        {
            get
            {
                return this.maxStaleLimit;
            }
            set
            {
                this.maxStaleLimit = value;
            }
        }

        /// <summary>The freshness lifetime, in seconds, that an HTTP client is willing to accept a response.</summary>
        /// <returns>Returns <see cref="T:System.TimeSpan" />.The time in seconds.</returns>
        public TimeSpan? MinFresh
        {
            get
            {
                return this.minFresh;
            }
            set
            {
                this.minFresh = value;
            }
        }

        /// <summary>Whether the origin server require revalidation of a cache entry on any subsequent use when the cache entry becomes stale.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the origin server requires revalidation of a cache entry on any subsequent use when the entry becomes stale; otherwise, false.</returns>
        public bool MustRevalidate
        {
            get
            {
                return this.mustRevalidate;
            }
            set
            {
                this.mustRevalidate = value;
            }
        }

        /// <summary>Whether an HTTP client is willing to accept a cached response.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the HTTP client is willing to accept a cached response; otherwise, false.</returns>
        public bool NoCache
        {
            get
            {
                return this.noCache;
            }
            set
            {
                this.noCache = value;
            }
        }

        /// <summary>A collection of fieldnames in the "no-cache" directive in a cache-control header field on an HTTP response.</summary>
        /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.A collection of fieldnames.</returns>
        public ICollection<string> NoCacheHeaders
        {
            get
            {
                if (this.noCacheHeaders == null)
                {
                    this.noCacheHeaders = new ObjectCollection<string>(checkIsValidToken);
                }
                return this.noCacheHeaders;
            }
        }

        /// <summary>Whether a cache must not store any part of either the HTTP request mressage or any response.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if a cache must not store any part of either the HTTP request mressage or any response; otherwise, false.</returns>
        public bool NoStore
        {
            get
            {
                return this.noStore;
            }
            set
            {
                this.noStore = value;
            }
        }

        /// <summary>Whether a cache or proxy must not change any aspect of the entity-body.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if a cache or proxy must not change any aspect of the entity-body; otherwise, false.</returns>
        public bool NoTransform
        {
            get
            {
                return this.noTransform;
            }
            set
            {
                this.noTransform = value;
            }
        }

        /// <summary>Whether a cache should either respond using a cached entry that is consistent with the other constraints of the HTTP request, or respond with a 504 (Gateway Timeout) status.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if a cache should either respond using a cached entry that is consistent with the other constraints of the HTTP request, or respond with a 504 (Gateway Timeout) status; otherwise, false.</returns>
        public bool OnlyIfCached
        {
            get
            {
                return this.onlyIfCached;
            }
            set
            {
                this.onlyIfCached = value;
            }
        }

        /// <summary>Whether all or part of the HTTP response message is intended for a single user and must not be cached by a shared cache.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the HTTP response message is intended for a single user and must not be cached by a shared cache; otherwise, false.</returns>
        public bool Private
        {
            get
            {
                return this.privateField;
            }
            set
            {
                this.privateField = value;
            }
        }

        /// <summary>A collection fieldnames in the "private" directive in a cache-control header field on an HTTP response.</summary>
        /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.A collection of fieldnames.</returns>
        public ICollection<string> PrivateHeaders
        {
            get
            {
                if (this.privateHeaders == null)
                {
                    this.privateHeaders = new ObjectCollection<string>(checkIsValidToken);
                }
                return this.privateHeaders;
            }
        }

        /// <summary>Whether the origin server require revalidation of a cache entry on any subsequent use when the cache entry becomes stale for shared user agent caches.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the origin server requires revalidation of a cache entry on any subsequent use when the entry becomes stale for shared user agent caches; otherwise, false.</returns>
        public bool ProxyRevalidate
        {
            get
            {
                return this.proxyRevalidate;
            }
            set
            {
                this.proxyRevalidate = value;
            }
        }

        /// <summary>Whether an HTTP response may be cached by any cache, even if it would normally be non-cacheable or cacheable only within a non- shared cache.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the HTTP response may be cached by any cache, even if it would normally be non-cacheable or cacheable only within a non- shared cache; otherwise, false.</returns>
        public bool Public
        {
            get
            {
                return this.publicField;
            }
            set
            {
                this.publicField = value;
            }
        }

        /// <summary>The shared maximum age, specified in seconds, in an HTTP response that overrides the "max-age" directive in a cache-control header or an Expires header for a shared cache.</summary>
        /// <returns>Returns <see cref="T:System.TimeSpan" />.The time in seconds.</returns>
        public TimeSpan? SharedMaxAge
        {
            get
            {
                return this.sharedMaxAge;
            }
            set
            {
                this.sharedMaxAge = value;
            }
        }
    }
}

