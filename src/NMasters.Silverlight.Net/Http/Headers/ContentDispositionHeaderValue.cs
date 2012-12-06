using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Http.Internal;
using NMasters.Silverlight.Net.Properties;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents the value of the Content-Disposition header.</summary>
    public class ContentDispositionHeaderValue //: ICloneable
    {
        private const string creationDate = "creation-date";
        private string dispositionType;
        private const string fileName = "filename";
        private const string fileNameStar = "filename*";
        private const string modificationDate = "modification-date";
        private const string name = "name";
        private ICollection<NameValueHeaderValue> parameters;
        private const string readDate = "read-date";
        private const string size = "size";

        internal ContentDispositionHeaderValue()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" /> class.</summary>
        /// <param name="source">A <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" />. </param>
        //protected ContentDispositionHeaderValue(ContentDispositionHeaderValue source)
        //{
        //    this.dispositionType = source.dispositionType;
        //    if (source.parameters != null)
        //    {
        //        foreach (NameValueHeaderValue value2 in source.parameters)
        //        {
        //            this.Parameters.Add((NameValueHeaderValue) ((ICloneable) value2).Clone());
        //        }
        //    }
        //}

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" /> class.</summary>
        /// <param name="dispositionType">A string that contains a <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" />.</param>
        public ContentDispositionHeaderValue(string dispositionType)
        {
            CheckDispositionTypeFormat(dispositionType, "dispositionType");
            this.dispositionType = dispositionType;
        }

        private static void CheckDispositionTypeFormat(string dispositionType, string parameterName)
        {
            string str;
            if (string.IsNullOrEmpty(dispositionType))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, parameterName);
            }
            if ((GetDispositionTypeExpressionLength(dispositionType, 0, out str) == 0) || (str.Length != dispositionType.Length))
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, new object[] { dispositionType }));
            }
        }

        //private string Encode5987(string input)
        //{
        //    StringBuilder builder = new StringBuilder("utf-8''");
        //    foreach (char ch in input)
        //    {
        //        if (ch > '\x007f')
        //        {
        //            foreach (byte num in Encoding.UTF8.GetBytes(ch.ToString()))
        //            {
        //                builder.Append(Uri.HexEscape((char) num));
        //            }
        //        }
        //        else if ((!HttpRuleParser.IsTokenChar(ch) || (ch == '*')) || ((ch == '\'') || (ch == '%')))
        //        {
        //            builder.Append(Uri.HexEscape(ch));
        //        }
        //        else
        //        {
        //            builder.Append(ch);
        //        }
        //    }
        //    return builder.ToString();
        //}

        private string EncodeAndQuoteMime(string input)
        {
            string str = input;
            bool flag = false;
            if (this.IsQuoted(str))
            {
                str = str.Substring(1, str.Length - 2);
                flag = true;
            }
            if (str.IndexOf("\"", 0, StringComparison.Ordinal) >= 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, new object[] { input }));
            }
            if (this.RequiresEncoding(str))
            {
                flag = true;
                str = this.EncodeMime(str);
            }
            else if (!flag && (HttpRuleParser.GetTokenLength(str, 0) != str.Length))
            {
                flag = true;
            }
            if (flag)
            {
                str = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", new object[] { str });
            }
            return str;
        }

        private string EncodeMime(string input)
        {
            string str = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
            return string.Format(CultureInfo.InvariantCulture, "=?utf-8?B?{0}?=", new object[] { str });
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" />  object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            ContentDispositionHeaderValue value2 = obj as ContentDispositionHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            return ((string.Compare(this.dispositionType, value2.dispositionType, StringComparison.OrdinalIgnoreCase) == 0) && HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this.parameters, value2.parameters));
        }

        private DateTimeOffset? GetDate(string parameter)
        {
            NameValueHeaderValue value2 = NameValueHeaderValue.Find(this.parameters, parameter);
            if (value2 != null)
            {
                DateTimeOffset offset;
                string str = value2.Value;
                if (this.IsQuoted(str))
                {
                    str = str.Substring(1, str.Length - 2);
                }
                if (HttpRuleParser.TryStringToDate(str, out offset))
                {
                    return new DateTimeOffset?(offset);
                }
            }
            return null;
        }

        private static int GetDispositionTypeExpressionLength(string input, int startIndex, out string dispositionType)
        {
            dispositionType = null;
            int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
            if (tokenLength == 0)
            {
                return 0;
            }
            dispositionType = input.Substring(startIndex, tokenLength);
            return tokenLength;
        }

        internal static int GetDispositionTypeLength(string input, int startIndex, out object parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            string dispositionType = null;
            int num = GetDispositionTypeExpressionLength(input, startIndex, out dispositionType);
            if (num == 0)
            {
                return 0;
            }
            int num2 = startIndex + num;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            ContentDispositionHeaderValue value2 = new ContentDispositionHeaderValue {
                dispositionType = dispositionType
            };
            if ((num2 < input.Length) && (input[num2] == ';'))
            {
                num2++;
                int num3 = NameValueHeaderValue.GetNameValueListLength(input, num2, ';', value2.Parameters);
                if (num3 == 0)
                {
                    return 0;
                }
                parsedValue = value2;
                return ((num2 + num3) - startIndex);
            }
            parsedValue = value2;
            return (num2 - startIndex);
        }

        /// <summary>Serves as a hash function for an  <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return (this.dispositionType.ToLowerInvariant().GetHashCode() ^ NameValueHeaderValue.GetHashCode(this.parameters));
        }

        //private string GetName(string parameter)
        //{
        //    string str;
        //    NameValueHeaderValue value2 = NameValueHeaderValue.Find(this.parameters, parameter);
        //    if (value2 == null)
        //    {
        //        return null;
        //    }
        //    if (parameter.EndsWith("*", StringComparison.Ordinal))
        //    {
        //        if (this.TryDecode5987(value2.Value, out str))
        //        {
        //            return str;
        //        }
        //        return null;
        //    }
        //    if (this.TryDecodeMime(value2.Value, out str))
        //    {
        //        return str;
        //    }
        //    return value2.Value;
        //}

        private bool IsQuoted(string value)
        {
            return (((value.Length > 1) && value.StartsWith("\"", StringComparison.Ordinal)) && value.EndsWith("\"", StringComparison.Ordinal));
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" />  instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" />  instance.</returns>
        /// <param name="input">A string that represents content disposition header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid content disposition header value information.</exception>
        public static ContentDispositionHeaderValue Parse(string input)
        {
            int index = 0;
            return (ContentDispositionHeaderValue) GenericHeaderParser.ContentDispositionParser.ParseValue(input, null, ref index);
        }

        private bool RequiresEncoding(string input)
        {
            foreach (char ch in input)
            {
                if (ch > '\x007f')
                {
                    return true;
                }
            }
            return false;
        }

        private void SetDate(string parameter, DateTimeOffset? date)
        {
            NameValueHeaderValue item = NameValueHeaderValue.Find(this.parameters, parameter);
            if (!date.HasValue)
            {
                if (item != null)
                {
                    this.parameters.Remove(item);
                }
            }
            else
            {
                string str = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", new object[] { HttpRuleParser.DateToString(date.Value) });
                if (item != null)
                {
                    item.Value = str;
                }
                else
                {
                    this.Parameters.Add(new NameValueHeaderValue(parameter, str));
                }
            }
        }

        //private void SetName(string parameter, string value)
        //{
        //    NameValueHeaderValue item = NameValueHeaderValue.Find(this.parameters, parameter);
        //    if (string.IsNullOrEmpty(value))
        //    {
        //        if (item != null)
        //        {
        //            this.parameters.Remove(item);
        //        }
        //    }
        //    else
        //    {
        //        string str = string.Empty;
        //        if (parameter.EndsWith("*", StringComparison.Ordinal))
        //        {
        //            str = this.Encode5987(value);
        //        }
        //        else
        //        {
        //            str = this.EncodeAndQuoteMime(value);
        //        }
        //        if (item != null)
        //        {
        //            item.Value = str;
        //        }
        //        else
        //        {
        //            this.Parameters.Add(new NameValueHeaderValue(parameter, str));
        //        }
        //    }
        //}

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" />  instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new ContentDispositionHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            return (this.dispositionType + NameValueHeaderValue.ToString(this.parameters, ';', true));
        }

        //private bool TryDecode5987(string input, out string output)
        //{
        //    output = null;
        //    string[] strArray = input.Split(new char[] { '\'' });
        //    if (strArray.Length != 3)
        //    {
        //        return false;
        //    }
        //    StringBuilder builder = new StringBuilder();
        //    try
        //    {
        //        Encoding encoding = Encoding.GetEncoding(strArray[0]);
        //        string pattern = strArray[2];
        //        byte[] bytes = new byte[pattern.Length];
        //        int count = 0;
        //        for (int i = 0; i < pattern.Length; i++)
        //        {
        //            if (Uri.IsHexEncoding(pattern, i))
        //            {
        //                bytes[count++] = (byte) Uri.HexUnescape(pattern, ref i);
        //                i--;
        //            }
        //            else
        //            {
        //                if (count > 0)
        //                {
        //                    builder.Append(encoding.GetString(bytes, 0, count));
        //                    count = 0;
        //                }
        //                builder.Append(pattern[i]);
        //            }
        //        }
        //        if (count > 0)
        //        {
        //            builder.Append(encoding.GetString(bytes, 0, count));
        //        }
        //    }
        //    catch (ArgumentException)
        //    {
        //        return false;
        //    }
        //    output = builder.ToString();
        //    return true;
        //}

        //private bool TryDecodeMime(string input, out string output)
        //{
        //    output = null;
        //    string str = input;
        //    if (this.IsQuoted(str) && (str.Length >= 10))
        //    {
        //        string[] strArray = str.Split(new char[] { '?' });
        //        if (((strArray.Length != 5) || (strArray[0] != "\"=")) || ((strArray[4] != "=\"") || (strArray[2].ToLowerInvariant() != "b")))
        //        {
        //            return false;
        //        }
        //        try
        //        {
        //            Encoding encoding = Encoding.GetEncoding(strArray[1]);
        //            byte[] bytes = Convert.FromBase64String(strArray[3]);
        //            output = encoding.GetString(bytes);
        //            return true;
        //        }
        //        catch (ArgumentException)
        //        {
        //        }
        //        catch (FormatException)
        //        {
        //        }
        //    }
        //    return false;
        //}

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out ContentDispositionHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (GenericHeaderParser.ContentDispositionParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (ContentDispositionHeaderValue) obj2;
                return true;
            }
            return false;
        }

        /// <summary>The date at which   the file was created.</summary>
        /// <returns>Returns <see cref="T:System.DateTimeOffset" />.The file creation date.</returns>
        public DateTimeOffset? CreationDate
        {
            get
            {
                return this.GetDate("creation-date");
            }
            set
            {
                this.SetDate("creation-date", value);
            }
        }

        /// <summary>The disposition type for a content body part.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The disposition type. </returns>
        public string DispositionType
        {
            get
            {
                return this.dispositionType;
            }
            set
            {
                CheckDispositionTypeFormat(value, "value");
                this.dispositionType = value;
            }
        }

        /// <summary>A suggestion for how to construct a filename for   storing the message payload to be used if the entity is   detached and stored in a separate file.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A suggested filename.</returns>
        //public string FileName
        //{
        //    get
        //    {
        //        return this.GetName("filename");
        //    }
        //    set
        //    {
        //        this.SetName("filename", value);
        //    }
        //}

        /// <summary>A suggestion for how to construct filenames for   storing message payloads to be used if the entities are    detached and stored in a separate files.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A suggested filename of the form filename*.</returns>
        //public string FileNameStar
        //{
        //    get
        //    {
        //        return this.GetName("filename*");
        //    }
        //    set
        //    {
        //        this.SetName("filename*", value);
        //    }
        //}

        /// <summary>The date at   which the file was last modified. </summary>
        /// <returns>Returns <see cref="T:System.DateTimeOffset" />.The file modification date.</returns>
        public DateTimeOffset? ModificationDate
        {
            get
            {
                return this.GetDate("modification-date");
            }
            set
            {
                this.SetDate("modification-date", value);
            }
        }

        /// <summary>The name for a content body part.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The name for the content body part.</returns>
        //public string Name
        //{
        //    get
        //    {
        //        return this.GetName("name");
        //    }
        //    set
        //    {
        //        this.SetName("name", value);
        //    }
        //}

        /// <summary>A set of parameters included the Content-Disposition header.</summary>
        /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.A collection of parameters. </returns>
        public ICollection<NameValueHeaderValue> Parameters
        {
            get
            {
                if (this.parameters == null)
                {
                    this.parameters = new ObjectCollection<NameValueHeaderValue>();
                }
                return this.parameters;
            }
        }

        /// <summary>The date the file was last read.</summary>
        /// <returns>Returns <see cref="T:System.DateTimeOffset" />.The last read date.</returns>
        public DateTimeOffset? ReadDate
        {
            get
            {
                return this.GetDate("read-date");
            }
            set
            {
                this.SetDate("read-date", value);
            }
        }

        /// <summary>The approximate size, in bytes, of the file. </summary>
        /// <returns>Returns <see cref="T:System.Int64" />.The approximate size, in bytes.</returns>
        public long? Size
        {
            get
            {
                ulong num;
                NameValueHeaderValue value2 = NameValueHeaderValue.Find(this.parameters, "size");
                if ((value2 != null) && ulong.TryParse(value2.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
                {
                    return new long?((long) num);
                }
                return null;
            }
            set
            {
                NameValueHeaderValue item = NameValueHeaderValue.Find(this.parameters, "size");
                if (!value.HasValue)
                {
                    if (item != null)
                    {
                        this.parameters.Remove(item);
                    }
                }
                else
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException("value");
                    }
                    if (item != null)
                    {
                        item.Value = value.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        string str = value.Value.ToString(CultureInfo.InvariantCulture);
                        this.parameters.Add(new NameValueHeaderValue("size", str));
                    }
                }
            }
        }
    }
}

