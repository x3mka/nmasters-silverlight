using System;
using System.Globalization;
using System.Text;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Http.Internal;
using NMasters.Silverlight.Net.Properties;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents the value of a Via header.</summary>
    public class ViaHeaderValue //: ICloneable
    {
        private string comment;
        private string protocolName;
        private string protocolVersion;
        private string receivedBy;

        private ViaHeaderValue()
        {
        }

        private ViaHeaderValue(ViaHeaderValue source)
        {
            this.protocolName = source.protocolName;
            this.protocolVersion = source.protocolVersion;
            this.receivedBy = source.receivedBy;
            this.comment = source.comment;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.ViaHeaderValue" /> class.</summary>
        /// <param name="protocolVersion">The protocol version of the received protocol.</param>
        /// <param name="receivedBy">The host and port that the request or response was received by.</param>
        public ViaHeaderValue(string protocolVersion, string receivedBy) : this(protocolVersion, receivedBy, null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.ViaHeaderValue" /> class.</summary>
        /// <param name="protocolVersion">The protocol version of the received protocol.</param>
        /// <param name="receivedBy">The host and port that the request or response was received by.</param>
        /// <param name="protocolName">The protocol name of the received protocol.</param>
        public ViaHeaderValue(string protocolVersion, string receivedBy, string protocolName) : this(protocolVersion, receivedBy, protocolName, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.ViaHeaderValue" /> class.</summary>
        /// <param name="protocolVersion">The protocol version of the received protocol.</param>
        /// <param name="receivedBy">The host and port that the request or response was received by.</param>
        /// <param name="protocolName">The protocol name of the received protocol.</param>
        /// <param name="comment">The comment field used to identify the software of the recipient proxy or gateway.</param>
        public ViaHeaderValue(string protocolVersion, string receivedBy, string protocolName, string comment)
        {
            HeaderUtilities.CheckValidToken(protocolVersion, "protocolVersion");
            CheckReceivedBy(receivedBy);
            if (!string.IsNullOrEmpty(protocolName))
            {
                HeaderUtilities.CheckValidToken(protocolName, "protocolName");
                this.protocolName = protocolName;
            }
            if (!string.IsNullOrEmpty(comment))
            {
                HeaderUtilities.CheckValidComment(comment, "comment");
                this.comment = comment;
            }
            this.protocolVersion = protocolVersion;
            this.receivedBy = receivedBy;
        }

        private static void CheckReceivedBy(string receivedBy)
        {
            if (string.IsNullOrEmpty(receivedBy))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, "receivedBy");
            }
            string host = null;
            if (HttpRuleParser.GetHostLength(receivedBy, 0, true, out host) != receivedBy.Length)
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, new object[] { receivedBy }));
            }
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ViaHeaderValue" />object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            ViaHeaderValue value2 = obj as ViaHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            return ((((string.Compare(this.protocolVersion, value2.protocolVersion, StringComparison.OrdinalIgnoreCase) == 0) && (string.Compare(this.receivedBy, value2.receivedBy, StringComparison.OrdinalIgnoreCase) == 0)) && (string.Compare(this.protocolName, value2.protocolName, StringComparison.OrdinalIgnoreCase) == 0)) && (string.CompareOrdinal(this.comment, value2.comment) == 0));
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.ViaHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.Returns a hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int num = this.protocolVersion.ToLowerInvariant().GetHashCode() ^ this.receivedBy.ToLowerInvariant().GetHashCode();
            if (!string.IsNullOrEmpty(this.protocolName))
            {
                num ^= this.protocolName.ToLowerInvariant().GetHashCode();
            }
            if (!string.IsNullOrEmpty(this.comment))
            {
                num ^= this.comment.GetHashCode();
            }
            return num;
        }

        private static int GetProtocolEndIndex(string input, int startIndex, out string protocolName, out string protocolVersion)
        {
            protocolName = null;
            protocolVersion = null;
            int num = startIndex;
            int tokenLength = HttpRuleParser.GetTokenLength(input, num);
            if (tokenLength == 0)
            {
                return 0;
            }
            num = startIndex + tokenLength;
            int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, num);
            num += whitespaceLength;
            if (num == input.Length)
            {
                return 0;
            }
            if (input[num] == '/')
            {
                protocolName = input.Substring(startIndex, tokenLength);
                num++;
                num += HttpRuleParser.GetWhitespaceLength(input, num);
                tokenLength = HttpRuleParser.GetTokenLength(input, num);
                if (tokenLength == 0)
                {
                    return 0;
                }
                protocolVersion = input.Substring(num, tokenLength);
                num += tokenLength;
                whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, num);
                num += whitespaceLength;
            }
            else
            {
                protocolVersion = input.Substring(startIndex, tokenLength);
            }
            if (whitespaceLength == 0)
            {
                return 0;
            }
            return num;
        }

        internal static int GetViaLength(string input, int startIndex, out object parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            string protocolName = null;
            string protocolVersion = null;
            int num = GetProtocolEndIndex(input, startIndex, out protocolName, out protocolVersion);
            if ((num == startIndex) || (num == input.Length))
            {
                return 0;
            }
            string host = null;
            int num2 = HttpRuleParser.GetHostLength(input, num, true, out host);
            if (num2 == 0)
            {
                return 0;
            }
            num += num2;
            num += HttpRuleParser.GetWhitespaceLength(input, num);
            string str4 = null;
            if ((num < input.Length) && (input[num] == '('))
            {
                int length = 0;
                if (HttpRuleParser.GetCommentLength(input, num, out length) != HttpParseResult.Parsed)
                {
                    return 0;
                }
                str4 = input.Substring(num, length);
                num += length;
                num += HttpRuleParser.GetWhitespaceLength(input, num);
            }
            ViaHeaderValue value2 = new ViaHeaderValue {
                protocolVersion = protocolVersion,
                protocolName = protocolName,
                receivedBy = host,
                comment = str4
            };
            parsedValue = value2;
            return (num - startIndex);
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.ViaHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.ViaHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.ViaHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents via header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid via header value information.</exception>
        public static ViaHeaderValue Parse(string input)
        {
            int index = 0;
            return (ViaHeaderValue) GenericHeaderParser.SingleValueViaParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ViaHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new ViaHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ViaHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrEmpty(this.protocolName))
            {
                builder.Append(this.protocolName);
                builder.Append('/');
            }
            builder.Append(this.protocolVersion);
            builder.Append(' ');
            builder.Append(this.receivedBy);
            if (!string.IsNullOrEmpty(this.comment))
            {
                builder.Append(' ');
                builder.Append(this.comment);
            }
            return builder.ToString();
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.ViaHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.ViaHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.ViaHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out ViaHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (GenericHeaderParser.SingleValueViaParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (ViaHeaderValue) obj2;
                return true;
            }
            return false;
        }

        /// <summary>Gets the comment field used to identify the software of the recipient proxy or gateway.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The comment field used to identify the software of the recipient proxy or gateway.</returns>
        public string Comment
        {
            get
            {
                return this.comment;
            }
        }

        /// <summary>Gets the protocol name of the received protocol.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The protocol name.</returns>
        public string ProtocolName
        {
            get
            {
                return this.protocolName;
            }
        }

        /// <summary>Gets the protocol version of the received protocol.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The protocol version.</returns>
        public string ProtocolVersion
        {
            get
            {
                return this.protocolVersion;
            }
        }

        /// <summary>Gets the host and port that the request or response was received by.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The host and port that the request or response was received by.</returns>
        public string ReceivedBy
        {
            get
            {
                return this.receivedBy;
            }
        }
    }
}

