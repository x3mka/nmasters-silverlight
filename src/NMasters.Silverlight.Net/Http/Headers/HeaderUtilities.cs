using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    internal static class HeaderUtilities
    {
        internal const string BytesUnit = "bytes";
        internal const string ConnectionClose = "close";
        internal static readonly NameValueWithParametersHeaderValue ExpectContinue = new NameValueWithParametersHeaderValue("100-continue");
        private const string qualityName = "q";
        internal static readonly Action<HttpHeaderValueCollection<string>, string> TokenValidator = ValidateToken;
        internal static readonly TransferCodingHeaderValue TransferEncodingChunked = new TransferCodingHeaderValue("chunked");

        internal static bool AreEqualCollections<T>(ICollection<T> x, ICollection<T> y)
        {
            return AreEqualCollections<T>(x, y, null);
        }

        internal static bool AreEqualCollections<T>(ICollection<T> x, ICollection<T> y, IEqualityComparer<T> comparer)
        {
            if (x == null)
            {
                if (y != null)
                {
                    return (y.Count == 0);
                }
                return true;
            }
            if (y == null)
            {
                return (x.Count == 0);
            }
            if (x.Count != y.Count)
            {
                return false;
            }
            if (x.Count != 0)
            {
                bool[] flagArray = new bool[x.Count];
                int index = 0;
                foreach (T local in x)
                {
                    index = 0;
                    bool flag = false;
                    foreach (T local2 in y)
                    {
                        if (!flagArray[index] && (((comparer == null) && local.Equals(local2)) || ((comparer != null) && comparer.Equals(local, local2))))
                        {
                            flagArray[index] = true;
                            flag = true;
                            break;
                        }
                        index++;
                    }
                    if (!flag)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static void CheckValidComment(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, parameterName);
            }
            int length = 0;
            if ((HttpRuleParser.GetCommentLength(value, 0, out length) != HttpParseResult.Parsed) || (length != value.Length))
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, new object[] { value }));
            }
        }

        internal static void CheckValidQuotedString(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, parameterName);
            }
            int length = 0;
            if ((HttpRuleParser.GetQuotedStringLength(value, 0, out length) != HttpParseResult.Parsed) || (length != value.Length))
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, new object[] { value }));
            }
        }

        internal static void CheckValidToken(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, parameterName);
            }
            if (HttpRuleParser.GetTokenLength(value, 0) != value.Length)
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, new object[] { value }));
            }
        }

        internal static string DumpHeaders(params HttpHeaders[] headers)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{\r\n");
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i] != null)
                {
                    foreach (KeyValuePair<string, IEnumerable<string>> pair in headers[i])
                    {
                        foreach (string str in pair.Value)
                        {
                            builder.Append("  ");
                            builder.Append(pair.Key);
                            builder.Append(": ");
                            builder.Append(str);
                            builder.Append("\r\n");
                        }
                    }
                }
            }
            builder.Append('}');
            return builder.ToString();
        }

        internal static DateTimeOffset? GetDateTimeOffsetValue(string headerName, HttpHeaders store)
        {
            object parsedValues = store.GetParsedValues(headerName);
            if (parsedValues != null)
            {
                return new DateTimeOffset?((DateTimeOffset) parsedValues);
            }
            return null;
        }

        internal static int GetNextNonEmptyOrWhitespaceIndex(string input, int startIndex, bool skipEmptyValues, out bool separatorFound)
        {
            separatorFound = false;
            int num = startIndex + HttpRuleParser.GetWhitespaceLength(input, startIndex);
            if ((num != input.Length) && (input[num] == ','))
            {
                separatorFound = true;
                num++;
                num += HttpRuleParser.GetWhitespaceLength(input, num);
                if (!skipEmptyValues)
                {
                    return num;
                }
                while ((num < input.Length) && (input[num] == ','))
                {
                    num++;
                    num += HttpRuleParser.GetWhitespaceLength(input, num);
                }
            }
            return num;
        }

        internal static double? GetQuality(ICollection<NameValueHeaderValue> parameters)
        {
            NameValueHeaderValue value2 = NameValueHeaderValue.Find(parameters, "q");
            if (value2 != null)
            {
                double result = 0.0;
                if (double.TryParse(value2.Value, NumberStyles.AllowDecimalPoint, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
                {
                    return new double?(result);
                }
                if (Logging.On)
                {
                    Logging.PrintError(Logging.Http, string.Format(CultureInfo.InvariantCulture, SR.net_http_log_headers_invalid_quality, new object[] { value2.Value }));
                }
            }
            return null;
        }

        internal static TimeSpan? GetTimeSpanValue(string headerName, HttpHeaders store)
        {
            object parsedValues = store.GetParsedValues(headerName);
            if (parsedValues != null)
            {
                return new TimeSpan?((TimeSpan) parsedValues);
            }
            return null;
        }        

        internal static void SetQuality(ICollection<NameValueHeaderValue> parameters, double? value)
        {
            NameValueHeaderValue item = NameValueHeaderValue.Find(parameters, "q");
            if (value.HasValue)
            {
                if ((value < 0.0) || (value > 1.0))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                string str = value.Value.ToString("0.0##", NumberFormatInfo.InvariantInfo);
                if (item != null)
                {
                    item.Value = str;
                }
                else
                {
                    parameters.Add(new NameValueHeaderValue("q", str));
                }
            }
            else if (item != null)
            {
                parameters.Remove(item);
            }
        }

        internal static bool TryParseInt32(string value, out int result)
        {
            return int.TryParse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out result);
        }

        internal static bool TryParseInt64(string value, out long result)
        {
            return long.TryParse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out result);
        }

        private static void ValidateToken(HttpHeaderValueCollection<string> collection, string value)
        {
            CheckValidToken(value, "item");
        }
    }
}

