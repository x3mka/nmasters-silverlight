using System;
using System.Text;

namespace NMasters.Silverlight.Net.Http.Headers
{
    internal class UriHeaderParser : HttpHeaderParser
    {
        internal static readonly UriHeaderParser RelativeOrAbsoluteUriParser = new UriHeaderParser(UriKind.RelativeOrAbsolute);
        private UriKind uriKind;

        private UriHeaderParser(UriKind uriKind) : base(false)
        {
            this.uriKind = uriKind;
        }

        internal static string DecodeUtf8FromString(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                bool flag = false;
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] > '\x00ff')
                    {
                        return input;
                    }
                    if (input[i] > '\x007f')
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    byte[] bytes = new byte[input.Length];
                    for (int j = 0; j < input.Length; j++)
                    {
                        if (input[j] > '\x00ff')
                        {
                            return input;
                        }
                        bytes[j] = (byte) input[j];
                    }
                    try
                    {
                        return Encoding.GetEncoding("utf-8").GetString(bytes, 0, input.Length);
                    }
                    catch (ArgumentException)
                    {
                    }
                }
            }
            return input;
        }

        public override string ToString(object value)
        {
            Uri uri = (Uri) value;
            if (uri.IsAbsoluteUri)
            {
                return uri.AbsoluteUri;
            }
            return uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
        }

        public override bool TryParseValue(string value, object storeValue, ref int index, out object parsedValue)
        {
            Uri uri;
            parsedValue = null;
            if (string.IsNullOrEmpty(value) || (index == value.Length))
            {
                return false;
            }
            string uriString = value;
            if (index > 0)
            {
                uriString = value.Substring(index);
            }
            if (!Uri.TryCreate(uriString, this.uriKind, out uri) && !Uri.TryCreate(DecodeUtf8FromString(uriString), this.uriKind, out uri))
            {
                return false;
            }
            index = value.Length;
            parsedValue = uri;
            return true;
        }
    }
}

